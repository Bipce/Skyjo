using LiteNetLib.Utils;
using Skyjo.Network.Enums;

namespace Skyjo.Network.Packets;

public sealed class SendWorldPacket : Packet
{
    public override PacketType Type => PacketType.SendWorld;
    private static NetworkManager NetworkManager => NetworkManager.Instance;

    public override void Serialize(NetDataWriter writer)
    {
        base.Serialize(writer);

        writer.Put(NetworkManager.Entities.Count);
        foreach (var entity in NetworkManager.Entities)
        {
            writer.Put(NetworkManager.GetEntityTypeId(entity.GetType()));
            writer.Put(entity.Id);
            writer.Put(entity.OwnerId);
        }

        foreach (var entity in NetworkManager.Entities)
        {
            entity.__SerializeReplicatedVars(writer);
        }
    }

    public override void Deserialize(NetDataReader reader)
    {
        var count = reader.GetInt();
        for (var i = 0; i < count; i++)
        {
            var entity = NetworkManager.CreateEntity(reader.GetByte());
            entity.Id = reader.GetInt();
            entity.OwnerId = reader.GetInt();
            NetworkManager.Entities.Add(entity);
        }

        foreach (var entity in NetworkManager.Entities)
        {
            entity.__DeserializeReplicatedVars(reader);
            entity.OnSpawned();
        }
    }
}