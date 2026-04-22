using Microsoft.Xna.Framework.Input;
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

    private bool IsKeyJustPressed(Keys key) => _keyboard.IsKeyDown(key) && _lastKeyboard.IsKeyUp(key);

    protected override void OnSpawned()
    {
        GameView.BindFunction<string, int[]>("selectCard", Server_OnPlayerSelectCard_PreMatch);
        GameView.InitGame(CardData.Empty, CardData.Empty);
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
        Multicast_StartGame(cards);
    }

    [Multicast]
    private void Multicast_StartGame(int[] cards)
    {
        _drawPile = new Stack<CardData>(cards.Select(x => new CardData { Number = x }));

        var discardedCard = _drawPile.Pop();
        discardedCard.IsRevealed = true;
        _discardPile.Push(discardedCard);

        GameView.InitGame(_drawPile.Peek(), _discardPile.Peek());

        foreach (var player in NetworkManager.GetEntities<Player>())
        {
            var newCards = GetPlayerCards();

            for (var i = 0; i < player.Data.Cards.Count; i++)
            {
                player.Data.Cards[i].Number = newCards[i].Number;
                if (player.Data.Cards[i].WillBeRevealed)
                    player.Data.Cards[i].IsRevealed = true;
            }

            GameView.UpdatePlayer(player.Username, player.Data);
        }
    }

    private CardData[] GetPlayerCards()
    {
        var cards = new CardData[12];
        for (var i = 0; i < cards.Length; i++)
            cards[i] = _drawPile.Pop();

        return cards;
    }

    [Server]
    private void Server_OnPlayerSelectCard_PreMatch(string username, int[] indexes)
    {
        if (indexes.Length == 0)
            return;

        var player = NetworkManager.GetEntities<Player>().First(x => x.Username == username);
        player.RevealedCardIndexes = indexes;
        player.OnRep_RevealedCardIndexes();
    }
}