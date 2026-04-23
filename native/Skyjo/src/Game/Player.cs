using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    private GameManager _gameManager = null!;
    [Replicated] public string Username { get; set; } = null!;

    [Replicated(OnRep = nameof(OnRep_Cards))]
    public Card[]? Cards { get; private set; }

    protected override void OnSpawned()
    {
        _gameManager = NetworkManager.GetEntity<GameManager>();

        if (HasAuthority)
        {
            var cards = Enumerable.Range(0, 12).Select(_ => new Card
            {
                Owner = Owner,
                Player = this,
                CardType = (int)Enums.CardType.Player
            }).ToArray();

            foreach (var card in cards)
            {
                card.Spawn();
            }

            Cards = cards;
            OnRep_Cards();
        }
    }

    protected override void OnDestroyed()
    {
        GameView.RemovePlayer(Username);
    }

    private void OnRep_Cards()
    {
        GameView.AddPlayer(Data);
    }

    public PlayerData Data =>
        new()
        {
            Id = Id,
            Username = Username,
            IsOwner = IsOwner,
            Cards = Cards!.Select(x => x.Data).ToArray()
        };
}