using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

internal sealed class CreateEntityPacket : Packet
{
    public NetDataReader Reader { get; set; } = null!;

    public CreateEntityPacket()
    {
    }

    public CreateEntityPacket(byte typeId, int id, int ownerId)
    {
        TypeId = typeId;
        Id = id;
        OwnerId = ownerId;
    }

    public override PacketType Type => PacketType.CreateEntity;

    public byte TypeId { get; private set; }
    public int Id { get; private set; }
    public int OwnerId { get; private set; }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(TypeId);
        writer.Put(Id);
        writer.Put(OwnerId);
    }

    public override void Deserialize(NetDataReader reader)
    {
        Reader = reader;

        TypeId = reader.GetByte();
        Id = reader.GetInt();
        OwnerId = reader.GetInt();
    }
}