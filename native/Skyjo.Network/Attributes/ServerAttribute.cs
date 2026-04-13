using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Skyjo.Network.Aspects;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerAttribute : OverrideMethodAspect
{
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

    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;

        var methodId = NetworkHelper.ComputeMethodId(meta.Target.Method);

        if (!networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            var writer = networkManager.ServerManager.GetRpcPacketData(entity.Id, methodId);

            foreach (var param in meta.Target.Method.Parameters)
            {
                writer.Put(param.Value);
            }

            networkManager.ClientManager.Send(writer);
            return null;
        }

        return meta.Proceed();
    }
}