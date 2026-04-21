using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo.Game;

public sealed partial class Player : Entity
{
    [Replicated] public string Username { get; set; } = null!;

    protected override void OnSpawned()
    {
        if (IsOwner)
        {
            GameView.EvaluateScript($"window.setUsername(\"{Username}\");");
        }
    }
}