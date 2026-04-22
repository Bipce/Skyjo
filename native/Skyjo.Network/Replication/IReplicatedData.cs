using LiteNetLib.Utils;

namespace Skyjo.Network.Replication;

public interface IReplicatedData
{
    Entity Entity { get; }
    byte Id { get; }
    bool IsUnchanged { get; }
    Action<NetDataWriter> Serialize { get; }
    Action Done { get; }
    bool IsValid { get; }
}