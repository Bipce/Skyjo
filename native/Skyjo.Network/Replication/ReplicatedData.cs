using LiteNetLib.Utils;

namespace Skyjo.Network.Replication;

public sealed class ReplicatedData<T> : IReplicatedData
{
    public required Entity Entity { get; init; }
    public required byte Id { get; init; }
    public required T LastValue { get; init; }
    public required T Value { get; set; }

    public bool IsUnchanged
    {
        get
        {
            if (Value is Entity { IsPendingDestroy: true })
                return true;

            return EqualityComparer<T>.Default.Equals(Value, LastValue);
        }
    }

    public Action<NetDataWriter> Serialize { get; set; } = null!;
    public Action Done { get; set; } = null!;

    public bool IsValid
    {
        get
        {
            if (Value is Entity entity)
                return entity.Id > 0; // entity is spawned

            return true;
        }
    }
}