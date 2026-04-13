using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed class GameManager : Entity
{
    public string Message => "Hello World!";
    
    [Server]
    public void Server_SpawnEntity()
    {
        new TestEntity().Spawn();
        // Multicast_SayHello();
    }

    [Multicast]
    private void Multicast_SayHello()
    {
        Console.WriteLine($"Hello: {Random.Shared.Next()}");
    }
}