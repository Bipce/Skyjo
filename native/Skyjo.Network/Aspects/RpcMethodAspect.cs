using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;

namespace Skyjo.Network.Aspects;

public abstract class RpcMethodAspect : OverrideMethodAspect
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
}
