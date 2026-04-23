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

    public static void UpdatePlayer(ushort id, PlayerData data)
    {
        EvaluateScript($"window.updatePlayer({id}, {data.Serialize()});");
    }

    public static void UpdateDrawnCard(CardData data)
    {
        EvaluateScript($"window.updateDrawnCard({data.Serialize()})");
    }

    public static void UpdateDiscardedCard(CardData data)
    {
        EvaluateScript($"window.updateDiscardedCard({data.Serialize()})");
    }
}