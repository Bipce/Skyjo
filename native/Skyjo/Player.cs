using Microsoft.Xna.Framework;
using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed class Player : Entity
{
    public Color Color { get; init; } // todo: Replicated

    [Server]
    public void Server_SpawnEntity()
    {
        new TestEntity().Spawn();
    }
}