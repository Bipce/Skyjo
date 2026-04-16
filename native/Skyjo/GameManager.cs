using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Replicated] private int _health = 100;
    private int _lastHealth;
    [Replicated] private TestEntity _entity = null!;

    public GameManager()
    {
        _lastHealth = _health;
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
        _health -= 10;
    }

    [Server]
    public void Server_IncrementHealth()
    {
        // Console.WriteLine("Health += 10");
        _health += 10;

        _entity = new TestEntity();
        _entity.Spawn();
    }

    public void Update()
    {
        if (_lastHealth != _health)
        {
            _lastHealth = _health;
            Console.WriteLine(_health);
        }

        if (_entity != null)
            Console.WriteLine(_entity.Id);
    }
}