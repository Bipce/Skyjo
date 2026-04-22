using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Metalama.Framework.Code.Types;
using Microsoft.Xna.Framework;
using Skyjo.Network.Extensions;

namespace Skyjo.Network.Utils;

internal sealed class NetworkTemplates : ITemplateProvider
{
    private static bool IsBasicType(IType type)
    {
        HashSet<string> types =
        [
            "byte", "sbyte", "short", "ushort", "int", "uint", "long", "ulong", "char", "string", "bool", "double",
            "float"
        ];
        return types.Contains(type.ToString()!);
    }

    [Template]
    public static void WriteType(IType type, NetDataWriter writer, dynamic value)
    {
        var isEntity = type.IsConvertibleTo(typeof(Entity));
        var isValid = true;

        if (type.IsNullable == true && !isEntity)
        {
            isValid = value != null;
            writer.Put(isValid);
        }

        var isNullable = type is { IsNullable: true, IsReferenceType: false };
        if (isValid)
        {
            if (type is IArrayType { ElementType: INamedType elementType })
            {
                writer.Put(value!.Length);
                for (var i = 0; i < value.Length; i++)
                    WriteType(elementType, writer, value[i]);
            }
            else if (IsBasicType(type.ToNonNullable()))
                writer.Put(isNullable ? value!.Value : value);
            else
                NetDataExtensions.Put(writer, isNullable ? value!.Value : value);
        }
    }

    [CompileTime]
    private static void ThrowUnsupportedType(IType type) =>
        throw new InvalidOperationException($"Unsupported type: {type}");

    [Template]
    private static void ReadValue(IType type, NetDataReader reader, IExpression field)
    {
        if (type.IsConvertibleTo(typeof(byte)))
            field.Value = reader.GetByte();
        else if (type.IsConvertibleTo(typeof(sbyte)))
            field.Value = reader.GetSByte();
        else if (type.IsConvertibleTo(typeof(short)))
            field.Value = reader.GetShort();
        else if (type.IsConvertibleTo(typeof(ushort)))
            field.Value = reader.GetUShort();
        else if (type.IsConvertibleTo(typeof(int)))
            field.Value = reader.GetInt();
        else if (type.IsConvertibleTo(typeof(uint)))
            field.Value = reader.GetUInt();
        else if (type.IsConvertibleTo(typeof(long)))
            field.Value = reader.GetLong();
        else if (type.IsConvertibleTo(typeof(ulong)))
            field.Value = reader.GetULong();
        else if (type.IsConvertibleTo(typeof(char)))
            field.Value = reader.GetChar();
        else if (type.IsConvertibleTo(typeof(string)))
            field.Value = reader.GetString();
        else if (type.IsConvertibleTo(typeof(bool)))
            field.Value = reader.GetBool();
        else if (type.IsConvertibleTo(typeof(double)))
            field.Value = reader.GetDouble();
        else if (type.IsConvertibleTo(typeof(float)))
            field.Value = reader.GetFloat();
        else if (type.IsConvertibleTo(typeof(Entity)))
            field.Value = meta.Cast(type, reader.GetEntity());
        else if (type.IsConvertibleTo(typeof(Color)))
            field.Value = reader.GetColor();
        else
            ThrowUnsupportedType(type);
    }

    [Template]
    public static void ReadType(IType type, NetDataReader reader, IExpression field)
    {
        field.Value = default;
        var isValid = true;
        var isEntity = type.IsConvertibleTo(typeof(Entity));

        if (type.IsNullable == true && !isEntity)
        {
            isValid = reader.GetBool();
        }

        if (isValid)
        {
            if (type is IArrayType { ElementType: INamedType elementType })
            {
                field.Value = ExpressionFactory.Parse($"new {elementType}[reader.GetInt()]");
                var element = meta.DefineLocalVariable("element", elementType);
                for (var i = 0; i < field.Value!.Length; i++)
                {
                    ReadType(elementType, reader, element);
                    field.Value![i] = element.Value;
                }
            }
            else
                ReadValue(type.ToNonNullable(), reader, field);
        }
    }
}