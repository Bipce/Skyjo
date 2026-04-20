using LiteNetLib.Utils;

namespace Skyjo.Network;

public interface IReplicatedData
{
    Entity Entity { get; }
    int Index { get; }
    bool IsUnchanged { get; }
    Action<NetDataWriter> Serialize { get; }
    Action Done { get; }
    bool IsValid { get; }
}