using LiteNetLib.Utils;
using Microsoft.Xna.Framework;

namespace Skyjo.Network.Extensions;

public static class NetDataExtensions
{
    public static void Put(this NetDataWriter writer, Entity? entity)
    {
        writer.Put(entity ? entity.Id : 0);
    }

    public static Entity? GetEntity(this NetDataReader reader)
    {
        var id = reader.GetUShort();
        return id == 0 ? null : NetworkManager.Instance.GetEntity(id);
    }

    public static void Put(this NetDataWriter writer, Color color)
    {
        writer.Put(color.R);
        writer.Put(color.G);
        writer.Put(color.B);
        writer.Put(color.A);
    }

    public static Color GetColor(this NetDataReader reader)
    {
        return new Color(reader.GetByte(), reader.GetByte(), reader.GetByte(), reader.GetByte());
    }
}