using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Server]
    public void Server_SpawnEntity()
    {
        new TestEntity().Spawn();
    }

    [Server]
    public void Server_SendMessageToPlayer()
    {
        var players = NetworkManager.GetEntities<Player>().ToArray();
        players[^1].Client_SayHello();
    }
}