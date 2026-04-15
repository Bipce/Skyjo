using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Types;
using Skyjo.Network.Extensions;

namespace Skyjo.Network.Utils;

[CompileTime]
internal static class NetworkHelper
{
    public static int ComputeMethodId(IMethod method)
    {
        var parameters = string.Join(",", method.Parameters.Select(x => x.Type.ToString()));
        var fullName = $"{method.DeclaringType.FullName}.{method.Name}({parameters})";
        unchecked
        {
            var hash = (int)2166136261;
            foreach (var c in fullName)
            {
                hash ^= c;
                hash *= 16777619;
            }

            return hash;
        }
    }

    public static string GetReaderExpression(IType type)
    {
        if (type is INamedType namedType && namedType.IsConvertibleTo(typeof(Entity)))
            return $"{typeof(NetDataExtensions).FullName}.GetEntity<{namedType.FullName}>(reader)";

        if (type is IArrayType { ElementType: INamedType elemType } && elemType.IsConvertibleTo(typeof(Entity)))
            return $"{typeof(NetDataExtensions).FullName}.GetEntityArray<{elemType.FullName}>(reader)";

        return type.ToString() switch
        {
            "int" => "reader.GetInt()",
            "int[]" => "reader.GetIntArray()",
            "bool" => "reader.GetBool()",
            "bool[]" => "reader.GetBoolArray()",
            "string" => "reader.GetString()",
            "string[]" => "reader.GetStringArray()",
            "float" => "reader.GetFloat()",
            "float[]" => "reader.GetFloatArray()",
            "double" => "reader.GetDouble()",
            "double[]" => "reader.GetDoubleArray()",
            "byte" => "reader.GetByte()",
            "byte[]" => $"{typeof(NetDataExtensions).FullName}.GetBytesWithIntLength(reader)",
            "short" => "reader.GetShort()",
            "short[]" => "reader.GetShortArray()",
            "long" => "reader.GetLong()",
            "long[]" => "reader.GetLongArray()",
            _ => throw new InvalidOperationException($"Unsupported RPC parameter type: {type}")
        };
    }

    public static string GetWriterExpression(IType type, string valueName)
    {
        if (type is INamedType namedType && namedType.IsConvertibleTo(typeof(Entity)))
            return $"{typeof(NetDataExtensions).FullName}.PutEntity(writer, {valueName})";

        if (type is IArrayType { ElementType: INamedType elemType } && elemType.IsConvertibleTo(typeof(Entity)))
            return $"{typeof(NetDataExtensions).FullName}.PutEntityArray(writer, {valueName})";

        switch (type.ToString())
        {
            case "byte[]":
                return $"{typeof(NetDataExtensions).FullName}.PutBytesWithIntLength(writer, {valueName})";
            case "int[]" or "bool[]" or "string[]" or "float[]" or "double[]" or "short[]" or "long[]":
                return $"writer.PutArray({valueName})";
            case "int" or "bool" or "string" or "float" or "double" or "byte" or "short" or "long":
                return $"writer.Put({valueName})";
            default:
                throw new InvalidOperationException($"Unsupported replicated property type: {type}");
        }
    }
}