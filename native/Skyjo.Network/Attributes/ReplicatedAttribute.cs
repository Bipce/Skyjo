using System.ComponentModel;
using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.DeclarationBuilders;
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

        var result = builder.With(builder.Target.DeclaringType).IntroduceField(
            $"__{builder.Target.Name}ReplicatedData",
            fieldType,
            IntroductionScope.Instance,
            OverrideStrategy.Ignore,
            fb =>
            {
                fb.Accessibility = Accessibility.Private;
                fb.AddAttribute(AttributeConstruction.Create(typeof(EditorBrowsableAttribute),
                    [EditorBrowsableState.Never]));
                fb.AddAttribute(
                    AttributeConstruction.Create(typeof(ObsoleteAttribute), [NetworkHelper.InternalMessage]));
            });

        _replicatedDataField = result.Declaration;

        builder.Outbound.Select(m => m.DeclaringType).RequireAspect<ReplicatedAspect>();
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

            var index = (int)meta.ThisType.__GetReplicatedVarIndex(meta.Target.FieldOrProperty.Name);
            var entity = (Entity)meta.This;

            if (_replicatedDataField!.Value == null)
            {
                var lastValue = meta.Target.FieldOrProperty.Value;
                meta.Proceed();
                _replicatedDataField.Value = networkManager.ServerManager.AddReplicatedData(entity.NetUpdateFrequency,
                    entity, index, lastValue, value);

                _replicatedDataField.Value.Serialize =
                    meta.RunTime<Action<NetDataWriter>>(writer =>
                    {
                        NetworkTemplates.WriteType(meta.Target.FieldOrProperty.Type, writer,
                            _replicatedDataField!.Value.Value);
                    });

                _replicatedDataField.Value.Done = meta.RunTime(() => { _replicatedDataField.Value = null; });
            }
            else
            {
                meta.Proceed();
                _replicatedDataField.Value.Value = value;
            }
        }
    }
}