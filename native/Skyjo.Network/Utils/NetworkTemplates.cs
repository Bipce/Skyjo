using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Types;
using Skyjo.Network.Extensions;

namespace Skyjo.Network.Utils;

internal sealed class NetworkTemplates : ITemplateProvider
{
    [Template]
    public static void WriteType(IType type, NetDataWriter writer, dynamic value)
    {
        if (type.IsConvertibleTo(typeof(Entity)))
            NetDataExtensions.PutEntity(writer, value);
        else if (type is IArrayType { ElementType: INamedType elemType } &&
                 elemType.IsConvertibleTo(typeof(Entity)))
            NetDataExtensions.PutEntityArray(writer, value);
        else if (type.ToString() == "byte[]")
            NetDataExtensions.PutBytesWithIntLength(writer, value);
        else
        {
            if (type.TypeKind == TypeKind.Array)
                writer.PutArray(value);
            else
                writer.Put(value);
        }
    }
}
