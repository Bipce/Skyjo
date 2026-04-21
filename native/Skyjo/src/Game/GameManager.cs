using System.Text.Json;
using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class GameManager : Entity
{
    private const int NumberOfCards = 150;
    private const int NumberOfMinosTwoCards = 5;
    private const int NumberOfZeroCards = 15;
    private const int NumberOfOtherCards = 10;

    [Replicated(OnRep = nameof(OnRep_RandomCards))]
    private int[] _randomCards = null!;

    private Stack<CardData> _drawPile = null!;
    private readonly Stack<CardData> _discardPile = [];

    protected override void OnSpawned()
    {
        if (HasAuthority)
        {
            InitCards();
        }
    }

    private void InitCards()
    {
        var data = new List<int>(NumberOfCards);
        for (var i = 0; i < NumberOfMinosTwoCards; i++)
            data.Add(-2);
        for (var i = 0; i < NumberOfZeroCards; i++)
            data.Add(0);

        for (var i = -1; i <= 12; i++)
        {
            if (i == 0)
                continue;

            for (var j = 0; j < NumberOfOtherCards; j++)
                data.Add(i);
        }

        _randomCards = data.Shuffle().ToArray();
        OnRep_RandomCards();
    }

    private void OnRep_RandomCards()
    {
        _drawPile = new Stack<CardData>(_randomCards.Select(x => new CardData { Number = x }));
        _discardPile.Push(_drawPile.Pop());
        InitGame();
    }

    public CardData[] GetPlayerCards()
    {
        var cards = new CardData[12];
        for (var i = 0; i < cards.Length; i++)
            cards[i] = _drawPile.Pop();

        return cards;
    }

    private void InitGame()
    {
        var drawCard = _drawPile.Peek().Serialize();
        var discardCard = _discardPile.Peek().Serialize();
        GameView.EvaluateScript($"window.initGame({drawCard}, {discardCard})");
    }
}