using LiteNetLib.Utils;

namespace Skyjo.Network.Extensions;

public static class NetDataExtensions
{
    public static void PutBytesWithIntLength(this NetDataWriter writer, byte[] data)
    {
        writer.Put(data.Length);
        writer.Put(data);
    }

    public static byte[] GetBytesWithIntLength(this NetDataReader reader)
    {
        var data = new byte[reader.GetInt()];
        reader.GetBytes(data, data.Length);
        return data;
    }

    public static void PutEntity(this NetDataWriter writer, Entity entity)
    {
        writer.Put(entity.Id);
    }

    public static T GetEntity<T>(this NetDataReader reader) where T : Entity
    {
        return NetworkManager.Instance.GetEntity<T>(reader.GetInt());
    }

    public static void PutEntityArray<T>(this NetDataWriter writer, T[] entities) where T : Entity
    {
        writer.Put(entities.Length);
        foreach (var entity in entities)
            writer.Put(entity.Id);
    }

    public static T[] GetEntityArray<T>(this NetDataReader reader) where T : Entity
    {
        var entities = new T[reader.GetInt()];
        for (var i = 0; i < entities.Length; i++)
        {
            entities[i] = NetworkManager.Instance.GetEntity<T>(reader.GetInt());
        }

        return entities;
    }
}