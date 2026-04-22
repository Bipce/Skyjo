using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Skyjo.Network.Enums;
using Skyjo.Network.Utils;

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
            NetworkTemplates.WriteType(param.Type, writer, param.Value);
        }
    }

    [CompileTime]
    protected static byte GetMethodId()
    {
        var methods = meta.Target.Type.Methods
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))));

        byte i = 0;
        foreach (var method in methods)
        {
            if (method == meta.Target.Method)
                return i;
            i++;
        }

        throw new InvalidOperationException($"Method {meta.Target.Method} not found");
    }
}