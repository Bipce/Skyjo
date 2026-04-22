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
            Cards = Enumerable.Range(0, 12).Select(_ => CardData.Empty).ToArray()
        };
        GameView.AddPlayer(player);
    }

    protected override void OnDestroyed()
    {
        GameView.RemovePlayer(Username);
    }
}