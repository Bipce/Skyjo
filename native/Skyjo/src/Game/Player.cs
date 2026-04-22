using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    private GameManager _gameManager = null!;
    [Replicated] public string Username { get; set; } = null!;
    public PlayerData Data { get; private set; } = null!;

    [Replicated(OnRep = nameof(OnRep_RevealedCardIndexes))]
    public int[]? RevealedCardIndexes { get; set; }

    protected override void OnSpawned()
    {
        _gameManager = NetworkManager.GetEntity<GameManager>();

        var cards = Enumerable.Range(0, 12).Select(_ => CardData.Empty).ToList();

        Data = new PlayerData
        {
            Username = Username,
            IsOwner = IsOwner,
            Cards = cards
        };
        GameView.AddPlayer(Data);
    }

    protected override void OnDestroyed()
    {
        GameView.RemovePlayer(Username);
    }

    public void OnRep_RevealedCardIndexes()
    {
        foreach (var card in Data.Cards)
        {
            card.WillBeRevealed = false;
        }

        foreach (var index in RevealedCardIndexes!)
        {
            Data.Cards[index].WillBeRevealed = true;
        }
    }
}