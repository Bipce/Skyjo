using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public sealed class ReplicatedPacket : Packet
{
    public NetDataReader Reader { get; private set; } = null!;

    public ReplicatedPacket()
    {
    }

    public ReplicatedPacket(ushort entityId, int id)
    {
        EntityId = entityId;
        Id = id;
    }

    public override PacketType Type => PacketType.Replicated;

    public ushort EntityId { get; private set; }
    public int Id { get; private set; }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(EntityId);
        writer.Put(Id);
    }

    public override void Deserialize(NetDataReader reader)
    {
        EntityId = reader.GetUShort();
        Id = reader.GetInt();

        Reader = reader;
    }
}