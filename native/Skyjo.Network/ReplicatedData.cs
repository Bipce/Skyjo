using LiteNetLib.Utils;

namespace Skyjo.Network;

public sealed class ReplicatedData<T> : IReplicatedData
{
    public required Entity Entity { get; init; }
    public required int Index { get; init; }
    public required T LastValue { get; init; }
    public required T Value { get; set; }
    public bool IsUnchanged => EqualityComparer<T>.Default.Equals(Value, LastValue);
    public Action<NetDataWriter> Serialize { get; set; } = null!;
    public Action Done { get; set; } = null!;

    public bool IsValid
    {
        get
        {
            if (Value is Entity entity)
                return entity.IsValid;

            return true;
        }
    }
}