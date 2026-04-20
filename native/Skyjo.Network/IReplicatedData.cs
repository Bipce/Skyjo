using LiteNetLib.Utils;

namespace Skyjo.Network;

public interface IReplicatedData
{
    Entity Entity { get; }
    int Id { get; }
    bool IsUnchanged { get; }
    Action<NetDataWriter> Serialize { get; }
    Action Done { get; }
    bool IsValid { get; }
}