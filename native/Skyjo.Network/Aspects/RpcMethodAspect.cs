using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.Types;
using Metalama.Framework.Eligibility;
using Skyjo.Network.Enums;
using Skyjo.Network.Extensions;

namespace Skyjo.Network.Aspects;

public abstract class RpcMethodAspect : OverrideMethodAspect
{
    public Reliability Reliability { get; init; } = Reliability.ReliableOrdered;
    public byte Channel { get; init; }

    public override void BuildEligibility(IEligibilityBuilder<IMethod> builder)
    {
        base.BuildEligibility(builder);
        builder.DeclaringType().MustSatisfy(
            x => x.IsConvertibleTo(typeof(Entity)),
            x => $"{x} must inherit from {nameof(Entity)}");
    }

    public override void BuildAspect(IAspectBuilder<IMethod> builder)
    {
        base.BuildAspect(builder);
        builder.Outbound
            .Select(m => m.DeclaringType)
            .RequireAspect<RpcDispatcherAspect>();
    }

    [Template]
    protected static void WriteParams(NetDataWriter writer)
    {
        foreach (var param in meta.Target.Method.Parameters)
        {
            if (param.Type.IsConvertibleTo(typeof(Entity)))
                NetDataExtensions.PutEntity(writer, param.Value);
            else if (param.Type is IArrayType { ElementType: INamedType elemType } &&
                     elemType.IsConvertibleTo(typeof(Entity)))
                NetDataExtensions.PutEntityArray(writer, param.Value);
            else if (param.Type.ToString() == "byte[]")
                NetDataExtensions.PutBytesWithIntLength(writer, param.Value);
            else
            {
                if (param.Type.TypeKind == TypeKind.Array)
                    writer.PutArray(param.Value);
                else
                    writer.Put(param.Value);
            }
        }
    }

    [CompileTime]
    protected static int GetMethodId()
    {
        var methods = meta.Target.Type.Methods
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))));

        var i = 0;
        foreach (var method in methods)
        {
            if (method == meta.Target.Method)
                return i;
            i++;
        }

        throw new Exception($"Method {meta.Target.Method} not found");
    }
}