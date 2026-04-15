using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public sealed class ReplicatedPacket : Packet
{
    public NetDataReader Reader { get; private set; } = null!;

    public ReplicatedPacket()
    {
    }

    public ReplicatedPacket(int entityId, int index)
    {
        EntityId = entityId;
        Index = index;
    }

    public override PacketType Type => PacketType.Replicated;

    public int EntityId { get; private set; }
    public int Index { get; private set; }

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(EntityId);
        writer.Put(Index);
    }

    public override void Deserialize(NetDataReader reader)
    {
        EntityId = reader.GetInt();
        Index = reader.GetInt();

        Reader = reader;
    }
}