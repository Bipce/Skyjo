using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Replicated] private int Health { get; set; } = 100;
    
    [Server]
    public void Server_SpawnEntity()
    {
        new TestEntity().Spawn();
    }

    [Server]
    public void Server_DecrementHealth()
    {
        Console.WriteLine("Health -= 10");
        Health -= 10;
    }

    [Server]
    public void Server_IncrementHealth()
    {
        Console.WriteLine("Health += 10");
        Health += 10;
    }
}