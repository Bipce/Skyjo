using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public abstract class Packet
{
    public abstract PacketType Type { get; }

    public virtual void Serialize(NetDataWriter writer)
    {
        writer.Put((byte)Type);
    }

    public virtual void Deserialize(NetDataReader reader)
    {
    }
}