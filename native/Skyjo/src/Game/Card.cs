using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Card : Entity
{
    [Replicated] public int Number { get; set; }
    [Replicated] public bool IsRevealed { get; set; }
    [Replicated] public bool WillBeRevealed { get; set; }
    [Replicated] public Player Player { get; set; } = null!;
    [Replicated] public bool IsSelected { get; set; }

    // ReSharper disable once NotAccessedField.Local
    [Replicated(OnRep = nameof(OnRep_UpdateValue))]
    private int _updateViewCount;

    [Replicated] public int CardType { get; set; }

    public CardData Data =>
        new()
        {
            Id = Id,
            Number = Number,
            IsRevealed = IsRevealed,
            IsSelected = IsSelected,
        };

    private void OnRep_UpdateValue()
    {
        switch (CardType)
        {
            case (int)Enums.CardType.Player:
                GameView.UpdatePlayer(Player.Id, Player.Data);
                break;
            case (int)Enums.CardType.Draw:
                GameView.UpdateDrawnCard(Data);
                break;
            case (int)Enums.CardType.Discard:
                GameView.UpdateDiscardedCard(Data);
                break;
        }
    }

    public void UpdateView()
    {
        _updateViewCount++;
        OnRep_UpdateValue();
    }
}