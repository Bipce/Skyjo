using LiteNetLib.Utils;
using Skyjo.Network.Enums;
using Skyjo.Network.Extensions;

namespace Skyjo.Network.Packets;

public sealed class ReplicatedAllPacket : Packet
{
    public ReplicatedAllPacket()
    {
    }

    public ReplicatedAllPacket(Entity entity)
    {
        _entity = entity;
    }

    private Entity _entity = null!;

    public override PacketType Type => PacketType.ReplicatedAll;

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(_entity);
        _entity.__SerializeReplicatedVars(writer);
    }

    public override void Deserialize(NetDataReader reader)
    {
        _entity = reader.GetEntity() ?? throw new NullReferenceException("Entity is null");
        _entity.__DeserializeReplicatedVars(reader);
    }
}