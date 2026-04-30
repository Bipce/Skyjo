using Skyjo.ViewData;

namespace Skyjo.Game;

partial class GameView
{
    public static void AddPlayer(PlayerData data)
    {
        View.EvaluateScript($"window.addPlayer({data.Serialize()});");
    }

    public static void RemovePlayer(int id)
    {
        View.EvaluateScript($"window.removePlayer({id});");
    }

    public static void UpdatePlayer(ushort id, PlayerData data)
    {
        View.EvaluateScript($"window.updatePlayer({id}, {data.Serialize()});");
    }

    public static void UpdateDrawnCard(CardData data)
    {
        View.EvaluateScript($"window.updateDrawnCard({data.Serialize()})");
    }

    public static void UpdateDiscardedCard(CardData data)
    {
        View.EvaluateScript($"window.updateDiscardedCard({data.Serialize()})");
    }

    public static void RoundOver(bool isGameOver)
    {
        View.EvaluateScript($"window.roundOver({isGameOver})");
    }
}