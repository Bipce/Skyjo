using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Eligibility;
using Skyjo.Network.Aspects;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Attributes;

public sealed class ReplicatedAttribute : OverrideFieldOrPropertyAspect
{
    private IField? _replicatedDataField;

    public override void BuildEligibility(IEligibilityBuilder<IFieldOrProperty> builder)
    {
        base.BuildEligibility(builder);
        builder.DeclaringType().MustSatisfy(
            x => x.IsConvertibleTo(typeof(Entity)),
            x => $"{x} must inherit from {nameof(Entity)}");
    }

    public override void BuildAspect(IAspectBuilder<IFieldOrProperty> builder)
    {
        base.BuildAspect(builder);

        var fieldType = ((INamedType)TypeFactory.GetType(typeof(ReplicatedData<>)))
            .WithTypeArguments(builder.Target.Type)
            .ToNullable();

        var result = builder.With(builder.Target.DeclaringType).IntroduceField($"{builder.Target.Name}ReplicatedData",
            fieldType, IntroductionScope.Instance, OverrideStrategy.Ignore);

        _replicatedDataField = result.Declaration;

        builder.Outbound
            .Select(m => m.DeclaringType)
            .RequireAspect<ReplicatedAspect>();
    }

    public override dynamic? OverrideProperty
    {
        get => meta.Proceed();
        set
        {
            var networkManager = NetworkManager.Instance;
            if (!networkManager.ServerManager.IsRunning ||
                !networkManager.ServerManager.HasRemotePeers(out _))
            {
                meta.Proceed();
                return;
            }

            var index = (int)meta.ThisType.GetReplicatedVarIndex(meta.Target.FieldOrProperty.Name);
            var entity = (Entity)meta.This;

            if (_replicatedDataField!.Value == null)
            {
                _replicatedDataField.Value = networkManager.ServerManager.AddReplicatedData(entity.NetUpdateFrequency,
                    entity, index, meta.Target.FieldOrProperty.Value, value);

                var writerExpr = NetworkHelper.GetWriterExpression(meta.Target.FieldOrProperty.Type,
                    $"{_replicatedDataField!.Name}.Value");

                meta.InsertStatement($$"""
                                       {{_replicatedDataField.Name}}.Serialize = (writer) => {
                                           {{writerExpr}};
                                       };
                                       """);

                meta.InsertStatement($$"""
                                       {{_replicatedDataField.Name}}.Done = () => {
                                         {{_replicatedDataField.Name}} = null;
                                       };
                                       """);
            }
            else
            {
                _replicatedDataField.Value.Value = value;
            }

            meta.Proceed();
        }
    }
}
