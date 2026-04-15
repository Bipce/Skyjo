using Metalama.Framework.Aspects;
using Metalama.Framework.Code;

namespace Skyjo.Network.Attributes;

public sealed class ReplicatedAttribute : OverrideFieldOrPropertyAspect
{
    private IField? _replicatedDataField;

    public override void BuildAspect(IAspectBuilder<IFieldOrProperty> builder)
    {
        base.BuildAspect(builder);

        var fieldType = ((INamedType)TypeFactory.GetType(typeof(ReplicatedData<>)))
            .WithTypeArguments(builder.Target.Type);

        var result = builder.With(builder.Target.DeclaringType).IntroduceField(
            builder.Target.Name + "ReplicatedData",
            fieldType,
            IntroductionScope.Instance,
            OverrideStrategy.Ignore);

        _replicatedDataField = result.Declaration;
    }

    public override dynamic? OverrideProperty
    {
        get => meta.Proceed();
        set
        {
            var index = GetPropertyIndex();
            var networkManager = NetworkManager.Instance;
            var entity = (Entity)meta.This;

            if (_replicatedDataField!.Value == null)
            {
                _replicatedDataField.Value = networkManager.ServerManager.AddReplicatedData(entity.NetUpdateFrequency,
                    entity, index, meta.Target.FieldOrProperty.Value, value);

                meta.InsertStatement($$"""
                                     {{_replicatedDataField.Name}}.Serialize = (writer) => {
                                         writer.Put({{_replicatedDataField.Name}}.Value);
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

    [CompileTime]
    private static int GetPropertyIndex()
    {
        var i = 0;
        foreach (var member in meta.Target.FieldOrProperty.DeclaringType.FieldsAndProperties)
        {
            foreach (var attr in member.Attributes)
            {
                if (attr.Type.IsConvertibleTo(typeof(ReplicatedAttribute)))
                {
                    if (member.Name == meta.Target.FieldOrProperty.Name)
                        return i;
                    i++;
                    break;
                }
            }
        }

        throw new InvalidOperationException($"Property {meta.Target.FieldOrProperty.Name} not found");
    }
}