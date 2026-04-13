using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public sealed class DestroyEntityPacket : Packet
{
    public override PacketType Type => PacketType.DestroyEntity;

    public DestroyEntityPacket()
    {
    }

    public DestroyEntityPacket(int id)
    {
        Id = id;
    }

    public int Id { get; private set; }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(Id);
    }

    public override void Deserialize(NetDataReader reader)
    {
        Id = reader.GetInt();
    }
}