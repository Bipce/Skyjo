using Skyjo.Network;

namespace Skyjo;

public sealed class TestEntity : Entity
{
    protected override void OnSpawned()
    {
        Console.WriteLine("TestEntity Spawned");
    }

    protected override void OnDestroyed()
    {
        Console.WriteLine("TestEntity Destroyed");
    }
}