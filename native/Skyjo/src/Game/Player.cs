using System.Text.Json;
using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    [Replicated] public string Username { get; set; } = null!;

    protected override void OnSpawned()
    {
        var player = new PlayerData { Username = Username, IsOwner = IsOwner };
        AddPlayer(player);
    }

    protected override void OnDestroyed()
    {
        RemovePlayer(Username);
    }

    private static void AddPlayer(PlayerData data)
    {
        var json = JsonSerializer.Serialize(data, AppJsonContext.Default.PlayerData);
        GameView.EvaluateScript($"window.addPlayer({json});");
    }

    private static void RemovePlayer(string username)
    {
        GameView.EvaluateScript($"window.removePlayer(\"{username}\");");
    }
}