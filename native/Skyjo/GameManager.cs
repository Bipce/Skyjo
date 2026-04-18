using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Replicated] private int _health = 100;
    [Replicated] private TestEntity? _entity;

    private int _lastHealth;
    private bool _exist;

    public GameManager()
    {
        _lastHealth = _health;
        View_SetHealth(_health);
    }

    [Server]
    public void Server_SpawnEntity()
    {
        _entity = new TestEntity();
        _entity.Spawn();
    }

    [Server]
    public void Server_DecrementHealth()
    {
        _health -= 10;
    }

    [Server]
    public void Server_IncrementHealth()
    {
        _health += 10;
    }

    public void Update()
    {
        if (_lastHealth != _health)
        {
            _lastHealth = _health;
            View_SetHealth(_health);
        }

        if (_entity != null && !_exist)
        {
            Console.WriteLine(_entity);
            _exist = true;
        }
    }

    private void View_SetHealth(int health)
    {
        Application.View.EvaluateScript($"window.SetHealth(\"{health}\")");
    }
}