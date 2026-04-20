using Skyjo.Network;
using Skyjo.Network.Attributes;

namespace Skyjo;

public sealed partial class GameManager : Entity
{
    [Replicated(OnRep = nameof(OnRep_Health))]
    private int _health = 100;

    [Replicated(OnRep = nameof(OnRep_TestEntity))]
    private TestEntity? _entity;

    public GameManager()
    {
        View_SetHealth(_health);
    }

    [Server]
    public void Server_SpawnEntity()
    {
        _entity = new TestEntity();
        _entity.Spawn();
        OnRep_TestEntity();
    }

    [Server]
    public void Server_DestroyEntity()
    {
        _entity?.Destroy();
        _entity = null;
        OnRep_TestEntity();
    }

    [Server]
    public void Server_DecrementHealth()
    {
        _health -= 10;
        OnRep_Health();
    }

    [Server]
    public void Server_IncrementHealth()
    {
        _health += 10;
        OnRep_Health();
    }

    public void Update()
    {
    }

    private void View_SetHealth(int health)
    {
        Application.View.EvaluateScript($"window.SetHealth(\"{health}\")");
    }

    private void OnRep_Health()
    {
        View_SetHealth(_health);
    }

    private void OnRep_TestEntity()
    {
        if (!_entity)
            Console.WriteLine("Entity is null or not valid");
        else if (_entity)
            Console.WriteLine($"Entity is valid: {_entity.Id}");
    }
}