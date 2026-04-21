using Skyjo.Network;
using Skyjo.Network.Attributes;
using Skyjo.ViewData;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    private GameManager _gameManager = null!;
    [Replicated] public string Username { get; set; } = null!;

    protected override void OnSpawned()
    {
        _gameManager = NetworkManager.GetEntity<GameManager>();

        var player = new PlayerData
        {
            Username = Username,
            IsOwner = IsOwner,
            Cards = _gameManager.GetPlayerCards()
        };
        AddPlayer(player);
    }

    protected override void OnDestroyed()
    {
        RemovePlayer(Username);
    }

    private static void AddPlayer(PlayerData data)
    {
        GameView.EvaluateScript($"window.addPlayer({data.Serialize()});");
    }

    private static void RemovePlayer(string username)
    {
        GameView.EvaluateScript($"window.removePlayer(\"{username}\");");
    }
}