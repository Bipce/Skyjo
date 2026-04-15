using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Replicated] private int Health { get; set; } = 100;

    private int _lastHealth;

    public GameManager()
    {
        _lastHealth = Health;
    }

    [Server]
    public void Server_SpawnEntity()
    {
        new TestEntity().Spawn();
    }

    [Server]
    public void Server_DecrementHealth()
    {
        // Console.WriteLine("Health -= 10");
        Health -= 10;
    }

    [Server]
    public void Server_IncrementHealth()
    {
        // Console.WriteLine("Health += 10");
        Health += 10;
    }

    public void Update()
    {
        if (_lastHealth != Health)
        {
            _lastHealth = Health;
            Console.WriteLine(Health);
        }
    }
}