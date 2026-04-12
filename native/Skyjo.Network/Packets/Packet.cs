using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public abstract class Packet
{
    public abstract PacketType Type { get; }
    
    public abstract void Serialize(NetDataWriter writer);
    public abstract void Deserialize(NetDataReader reader);
}