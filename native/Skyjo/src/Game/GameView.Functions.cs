using Skyjo.ViewData;

namespace Skyjo.Game;

partial class GameView
{
    public static void AddPlayer(PlayerData data)
    {
        EvaluateScript($"window.addPlayer({data.Serialize()});");
    }

    public static void RemovePlayer(string username)
    {
        EvaluateScript($"window.removePlayer(\"{username}\");");
    }

    public static void InitGame(CardData drawnCard, CardData discardedCard)
    {
        EvaluateScript($"window.initGame({drawnCard.Serialize()}, {discardedCard.Serialize()});");
    }

    public static void UpdatePlayer(string username, PlayerData data)
    {
        EvaluateScript($"window.updatePlayer(\"{username}\", {data.Serialize()});");
    }
}