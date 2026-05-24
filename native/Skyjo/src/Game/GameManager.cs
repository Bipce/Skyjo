using System.Diagnostics.CodeAnalysis;
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

    private KeyboardState _keyboard;
    private KeyboardState _lastKeyboard;

    [Replicated(OnRep = nameof(OnRep_DrawnCard))]
    private Card _drawnCard = null!;

    [Replicated(OnRep = nameof(OnRep_DiscardedCard))]
    private Card _discardedCard = null!;

    [Replicated] public bool GameHasStarted { get; private set; }
    private bool _needToRevealCard;

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    private Player[] _players = null!;
    private Player _currentPlayer = null!;
    private int _currentPlayerIndex;

    private Player? _endGamePlayer;
    private bool _isGameEnded;

    protected override void OnSpawned()
    {
        GameView.View.BindFunction<ushort, ushort>("selectCard", Server_SelectCard);
        GameView.View.BindFunction<ushort, ushort, ushort>("dropCard", Server_DropCard);

        if (HasAuthority)
        {
            _drawnCard = new Card { Type = (int)CardType.Draw };
            _drawnCard.Spawn();
            _discardedCard = new Card { Type = (int)CardType.Discard };
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

    private void Reset()
    {
        _drawnCard.IsRevealed = false;
        _drawnCard.UpdateView();
        _discardedCard.IsRevealed = false;
        _discardedCard.UpdateView();

        var needResetTotalScore = _players.Any(x => x.TotalScore >= 100);

        foreach (var player in _players)
        {
            GivePlayerCards(player);

            foreach (var card in player.Cards!)
            {
                card.IsRevealed = false;
                card.IsHighlighted = false;
            }

            if (needResetTotalScore)
                player.TotalScore = 0;

            player.HasDoublePoint = false;

            player.UpdateScore();
            player.UpdateView();
        }

        GameHasStarted = false;
        _isGameEnded = false;
    }

    private void StartGame()
    {
        if (_isGameEnded)
        {
            Reset();
            return;
        }

        if (GameHasStarted)
            return;

        _players = NetworkManager.GetEntities<Player>().ToArray();
        if (_players.Any(p => p.Cards!.Count(c => c.IsSelected) != 2))
            return;

        GenerateDrawnCards();

        _drawnCard.Number = _drawPile.Pop().Number;
        _drawnCard.UpdateView();

        _discardedCard.Number = _drawPile.Pop().Number;
        _discardedCard.IsRevealed = true;
        _discardedCard.UpdateView();

        foreach (var player in _players)
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

            player.UpdateScore();
        }

        var maxScore = _players.Max(x => x.CurrentScore);
        var playersWithHighScore = _players.Where(x => x.CurrentScore == maxScore);
        _currentPlayer = playersWithHighScore.OrderBy(_ => Random.Shared.Next()).First();
        _currentPlayer.IsCurrentPlayer = true;
        _currentPlayerIndex = _players.IndexOf(_currentPlayer);

        UpdatePlayersView();

        GameHasStarted = true;
    }

    private void GenerateDrawnCards()
    {
        var data = new List<sbyte>(NumberOfCards);
        for (var i = 0; i < NumberOfMinosTwoCards; i++)
            data.Add(-2);
        for (var i = 0; i < NumberOfZeroCards; i++)
            data.Add(0);

        for (var i = -1; i <= 12; i++)
        {
            if (i == 0)
                continue;

            for (var j = 0; j < NumberOfOtherCards; j++)
                data.Add((sbyte)i);
        }

        _drawPile = new Stack<CardData>(data.Shuffle().Select(x => new CardData { Number = x }));
    }

    private void GivePlayerCards(Player player)
    {
        var data = player.Cards!.ToList();
        var delta = Player.NumberOfCards - data.Count;
        for (var i = 0; i < delta; i++)
        {
            var card = new Card
            {
                Owner = player.Owner,
                Player = player,
                Type = (int)CardType.Player
            };

            card.Spawn();
            data.Add(card);
        }

        if (delta > 0)
            player.Cards = data.ToArray();
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
        var cards = new CardData[Player.NumberOfCards];
        for (var i = 0; i < cards.Length; i++)
            cards[i] = _drawPile.Pop();

        return cards;
    }

    [Server]
    private void Server_SelectCard(ushort playerId, ushort cardId)
    {
        if (_isGameEnded)
            return;

        var card = NetworkManager.GetEntity<Card>(cardId);
        var cardType = (CardType)card.Type;

        if (!GameHasStarted && cardType == CardType.Player)
        {
            if (!card.IsSelected)
            {
                if (card.Player.Cards!.Count(x => x.IsSelected) == 2)
                    return;
            }

            card.IsSelected = !card.IsSelected;
            card.UpdateView();
            return;
        }

        if (GameHasStarted)
        {
            var player = NetworkManager.GetEntity<Player>(playerId);
            if (player != _currentPlayer)
                return;

            if (!_needToRevealCard && cardType == CardType.Draw)
            {
                card.IsRevealed = true;
                card.UpdateView();
            }
            else if (!_needToRevealCard && cardType == CardType.Player && !card.IsRevealed && _drawnCard.IsRevealed)
            {
                _discardedCard.Number = _drawnCard.Number;
                _discardedCard.UpdateView();
                _drawnCard.IsRevealed = false;
                _drawnCard.Number = GetDrawnCardNumber();
                _drawnCard.UpdateView();
                card.IsRevealed = true;
                card.IsHighlighted = true;
                card.UpdateView();
                player.UpdateScore();
                CheckCardsSameColumn(_currentPlayer);
                NextPlayer();
            }
            else if (_needToRevealCard && cardType == CardType.Player && !card.IsRevealed)
            {
                card.IsRevealed = true;
                card.IsHighlighted = true;
                card.UpdateView();
                _needToRevealCard = false;
                _discardedCard.IsHighlighted = false;
                _discardedCard.UpdateView();
                player.UpdateScore();
                CheckCardsSameColumn(_currentPlayer);
                NextPlayer();
            }
        }
    }

    [Server]
    private void Server_DropCard(ushort playerId, ushort sourceId, ushort targetId)
    {
        if (_isGameEnded)
            return;

        var player = NetworkManager.GetEntity<Player>(playerId);
        if (!player.IsCurrentPlayer || _needToRevealCard)
            return;

        var sourceCard = NetworkManager.GetEntity<Card>(sourceId);
        var targetCard = NetworkManager.GetEntity<Card>(targetId);

        var lastTargetNumber = targetCard.Number;
        (sourceCard.Number, targetCard.Number) = (targetCard.Number, sourceCard.Number);
        targetCard.IsRevealed = true;

        if (sourceCard.Type == (int)CardType.Draw)
        {
            sourceCard.IsRevealed = false;
            sourceCard.Number = GetDrawnCardNumber();

            if (targetCard.Type == (int)CardType.Player)
            {
                _discardedCard.Number = lastTargetNumber;
                _discardedCard.UpdateView();
            }
        }

        targetCard.IsHighlighted = true;

        player.UpdateScore();
        sourceCard.UpdateView();
        targetCard.UpdateView();

        if (sourceCard.Type == (int)CardType.Draw && targetCard.Type == (int)CardType.Discard)
        {
            _needToRevealCard = true;
            return;
        }

        CheckCardsSameColumn(_currentPlayer);
        NextPlayer();
    }

    private sbyte GetDrawnCardNumber()
    {
        if (_drawPile.TryPop(out var card))
        {
            return card.Number;
        }

        GenerateDrawnCards();
        return _drawPile.Pop().Number;
    }

    private void CheckCardsSameColumn(Player player)
    {
        if (IsCardsSameColumn(player, out var cards))
        {
            foreach (var card in cards)
            {
                card.Destroy();
            }

            player.Cards = player.Cards!.Where(x => !x.IsPendingDestroy).ToArray();
            player.UpdateScore();
            _discardedCard.Number = cards.First().Number;
            _discardedCard.UpdateView();
        }
    }

    private void NextPlayer()
    {
        if (!_endGamePlayer && _currentPlayer.Cards!.All(x => x.IsRevealed))
            _endGamePlayer = _currentPlayer;

        _currentPlayer.IsCurrentPlayer = false;
        _currentPlayerIndex++;
        if (_currentPlayerIndex == _players.Length)
            _currentPlayerIndex = 0;
        _currentPlayer = _players[_currentPlayerIndex];
        _currentPlayer.IsCurrentPlayer = true;

        foreach (var cards in _currentPlayer.Cards!)
            cards.IsHighlighted = false;

        CheckEndGame();
        UpdatePlayersView();
    }

    private void CheckEndGame()
    {
        if (_currentPlayer != _endGamePlayer)
            return;

        _currentPlayer.IsCurrentPlayer = false;
        foreach (var player in _players)
        {
            foreach (var card in player.Cards!)
            {
                if (!card.IsRevealed)
                {
                    card.IsRevealed = true;
                    card.IsHighlighted = true;
                    CheckCardsSameColumn(player);
                }
            }

            player.UpdateScore();
        }

        UpdateTotalPlayersScore();

        _isGameEnded = true;
        _endGamePlayer = null;

        var isGameOver = _players.Any(x => x.TotalScore >= 100);
        Multicast_RoundOver(isGameOver);
    }

    [Multicast]
    private void Multicast_RoundOver(bool isGameOver)
    {
        GameView.RoundOver(isGameOver);
    }

    private void UpdateTotalPlayersScore()
    {
        Player? playerMinScore = null;
        try
        {
            var minScore = _players.Min(x => x.CurrentScore);
            playerMinScore = _players.SingleOrDefault(x => x.CurrentScore == minScore);
        }
        catch
        {
            // ignored
        }
        finally
        {
            if (_endGamePlayer != playerMinScore)
            {
                _endGamePlayer!.TotalScore += (byte)_endGamePlayer.CurrentScore;
                _endGamePlayer.HasDoublePoint = true;
            }
        }

        foreach (var player in _players)
        {
            player.TotalScore += (byte)player.CurrentScore;
        }
    }

    private void UpdatePlayersView()
    {
        foreach (var player in _players)
            player.UpdateView();
    }

    private bool IsCardsSameColumn(Player player, [NotNullWhen(true)] out Card[]? cards)
    {
        var increment = player.Cards!.Length / 3;
        var currentCards = player.Cards;

        for (var i = 0; i < increment; i++)
        {
            if (!currentCards[i].IsRevealed || !currentCards[i + increment].IsRevealed ||
                !currentCards[i + increment * 2].IsRevealed)
                continue;
            if (currentCards[i].Number == currentCards[i + increment].Number &&
                currentCards[i].Number == currentCards[i + increment * 2].Number)
            {
                cards = [currentCards[i], currentCards[i + increment], currentCards[i + increment * 2]];
                return true;
            }
        }

        cards = null;
        return false;
    }
}