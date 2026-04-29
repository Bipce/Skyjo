using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    public const int NumberOfCards = 12;

    private GameManager _gameManager = null!;
    [Replicated] public string Username { get; set; } = null!;

    [Replicated(OnRep = nameof(OnRep_Cards))]
    public Card[]? Cards { get; set; }

    [Replicated] public short CurrentScore { get; set; }
    [Replicated] public byte TotalScore { get; set; }

    [Replicated] public bool IsCurrentPlayer { get; set; }

    // ReSharper disable once NotAccessedField.Local
    [Replicated(OnRep = nameof(OnRep_UpdateValue))]
    private int _updateViewCount;

    [Replicated] public bool HasDoublePoint { get; set; }

    protected override void OnSpawned()
    {
        _gameManager = NetworkManager.GetEntity<GameManager>();

        if (HasAuthority)
        {
            var cards = Enumerable.Range(0, NumberOfCards).Select(_ => new Card
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
        GameView.RemovePlayer(Id);
    }

    private void OnRep_Cards()
    {
        _gameManager = NetworkManager.GetEntity<GameManager>();

        if (!_gameManager.GameHasStarted)
        {
            GameView.AddPlayer(Data);
        }
    }

    public PlayerData Data =>
        new()
        {
            Id = Id,
            Username = Username,
            IsOwner = IsOwner,
            Cards = Cards!.Select(x => x.Data).ToArray(),
            CurrentScore = CurrentScore,
            TotalScore = TotalScore,
            IsCurrentPlayer = IsCurrentPlayer,
            HasDoublePoint = HasDoublePoint,
        };

    private void OnRep_UpdateValue()
    {
        GameView.UpdatePlayer(Id, Data);
    }

    public void UpdateView()
    {
        _updateViewCount++;
        OnRep_UpdateValue();
    }

    public void UpdateScore()
    {
        CurrentScore = (short)Cards!.Where(x => x.IsRevealed).Sum(x => x.Number);
    }
}