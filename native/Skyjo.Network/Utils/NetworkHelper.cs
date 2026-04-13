using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

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
            return $"NetworkManager.GetEntity<{namedType.FullName}>(reader.GetInt())";

        return type.ToString() switch
        {
            "int" => "reader.GetInt()",
            "bool" => "reader.GetBool()",
            "string" => "reader.GetString()",
            "float" => "reader.GetFloat()",
            "double" => "reader.GetDouble()",
            "byte" => "reader.GetByte()",
            "short" => "reader.GetShort()",
            "long" => "reader.GetLong()",
            _ => throw new InvalidOperationException($"Unsupported RPC parameter type: {type}")
        };
    }
}