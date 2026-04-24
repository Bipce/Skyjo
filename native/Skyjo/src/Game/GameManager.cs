using Microsoft.Xna.Framework.Input;
using Skyjo.Enums;
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

    private Stack<CardData> _drawPile = null!;
    private readonly Stack<CardData> _discardPile = [];

    private KeyboardState _keyboard;
    private KeyboardState _lastKeyboard;

    [Replicated(OnRep = nameof(OnRep_DrawnCard))]
    private Card _drawnCard = null!;

    [Replicated(OnRep = nameof(OnRep_DiscardedCard))]
    private Card _discardedCard = null!;

    [Replicated] private bool _gameHasStarted;

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    protected override void OnSpawned()
    {
        GameView.View.BindFunction<ushort>("selectCard", Server_SelectCard);
        GameView.View.BindFunction<ushort, ushort>("dropCard", Server_DropCard);

        if (HasAuthority)
        {
            _drawnCard = new Card { CardType = (int)CardType.Draw };
            _drawnCard.Spawn();
            _discardedCard = new Card { CardType = (int)CardType.Discard };
            _discardedCard.Spawn();

            OnRep_DrawnCard();
            OnRep_DiscardedCard();
        }
    }

    public void Update()
    {
        _lastKeyboard = _keyboard;
        _keyboard = Keyboard.GetState();

        if (IsKeyJustPressed(Keys.Enter))
        {
            StartGame();
        }
    }

    private void StartGame()
    {
        var players = NetworkManager.GetEntities<Player>().ToArray();
        if (players.Any(p => p.Cards!.Count(c => c.IsSelected) != 2))
            return;

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

        var cards = data.Shuffle().ToArray();

        _drawPile = new Stack<CardData>(cards.Select(x => new CardData { Number = x }));
        _discardPile.Push(_drawPile.Pop());

        _drawnCard.Number = _drawPile.Peek().Number;
        _drawnCard.UpdateView();

        _discardedCard.Number = _discardPile.Peek().Number;
        _discardedCard.IsRevealed = true;
        _discardedCard.UpdateView();

        foreach (var player in players)
        {
            var newCards = GetPlayerCards();
            for (var i = 0; i < player.Cards!.Length; i++)
            {
                player.Cards[i].Number = newCards[i].Number;
                if (player.Cards[i].IsSelected)
                {
                    player.Cards[i].IsRevealed = true;
                    player.Cards[i].IsSelected = false;
                }
            }

            player.CurrentScore = (byte)player.Cards.Where(x => x.IsRevealed).Sum(x => x.Number);
        }

        var maxScore = players.Max(x => x.CurrentScore);
        var playersWithHighScore = players.Where(x => x.CurrentScore == maxScore);
        var randomPlayer = playersWithHighScore.OrderBy(_ => Random.Shared.Next()).First();
        randomPlayer.IsCurrentPlayer = true;

        foreach (var player in players)
            player.UpdateView();

        _gameHasStarted = true;
    }

    private void OnRep_DrawnCard()
    {
        GameView.UpdateDrawnCard(_drawnCard.Data);
    }

    private void OnRep_DiscardedCard()
    {
        GameView.UpdateDiscardedCard(_discardedCard.Data);
    }

    private CardData[] GetPlayerCards()
    {
        var cards = new CardData[12];
        for (var i = 0; i < cards.Length; i++)
            cards[i] = _drawPile.Pop();

        return cards;
    }

    [Server]
    private void Server_SelectCard(ushort cardId)
    {
        var cardEntity = NetworkManager.GetEntity<Card>(cardId);
        var cardType = (CardType)cardEntity.CardType;

        if (!_gameHasStarted && cardType == CardType.Player)
        {
            if (!cardEntity.IsSelected)
            {
                if (cardEntity.Player.Cards!.Count(x => x.IsSelected) == 2)
                    return;
            }

            cardEntity.IsSelected = !cardEntity.IsSelected;
            cardEntity.UpdateView();
            return;
        }

        if (_gameHasStarted && cardType == CardType.Draw)
        {
            cardEntity.IsRevealed = true;
            cardEntity.UpdateView();
        }
    }

    [Server]
    private void Server_DropCard(ushort sourceId, ushort targetId)
    {
        var sourceCard = NetworkManager.GetEntity<Card>(sourceId);
        var targetCard = NetworkManager.GetEntity<Card>(targetId);

        (sourceCard.Number, targetCard.Number) = (targetCard.Number, sourceCard.Number);
        targetCard.IsRevealed = true;
        sourceCard.UpdateView();
        targetCard.UpdateView();
    }
}