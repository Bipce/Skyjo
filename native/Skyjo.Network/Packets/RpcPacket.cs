using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public sealed class RpcPacket : Packet
{
    public NetDataReader Reader { get; private set; } = null!;
    
    public RpcPacket()
    {
    }

    public RpcPacket(ushort entityId, int methodId)
    {
        EntityId = entityId;
        MethodId = methodId;
    }

    public override PacketType Type => PacketType.Rpc;

    public ushort EntityId { get; private set; }
    public int MethodId { get; private set; }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(EntityId);
        writer.Put(MethodId);
    }

    public override void Deserialize(NetDataReader reader)
    {
        EntityId = reader.GetUShort();
        MethodId = reader.GetInt();
        Reader = reader;
    }
}