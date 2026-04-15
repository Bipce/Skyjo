using LiteNetLib.Utils;

namespace Skyjo.Network;

public sealed class ReplicatedData<T> : IReplicatedData where T : IEquatable<T>
{
    public required Entity Entity { get; init; }
    public required int Index { get; init; }
    public required T LastValue { get; init; }
    public required T Value { get; set; }
    public bool IsUnchanged => Value.Equals(LastValue);
    public Action<NetDataWriter> Serialize { get; set; } = null!;
    public Action Done { get; set; } = null!;
}