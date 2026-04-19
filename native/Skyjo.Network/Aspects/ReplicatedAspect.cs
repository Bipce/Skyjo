using System.ComponentModel;
using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Skyjo.Network.Attributes;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

public sealed class ReplicatedAspect : TypeAspect
{
    [Introduce(Accessibility = Accessibility.Private, WhenExists = OverrideStrategy.Ignore)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    private static int __GetReplicatedVarIndex(string name)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        var switchBuilder = new SwitchStatementBuilder(ExpressionFactory.Capture(name));
        var i = meta.CompileTime(0);
        foreach (var replicatedVar in replicatedVars)
        {
            var label = SwitchStatementLabel.CreateLiteral(replicatedVar.Name);
            switchBuilder.AddCase(label,
                StatementFactory.Parse($"return {i};").UnwrapBlock());
            i++;
        }

        switchBuilder.AddDefault(StatementFactory.Parse("throw new global::System.InvalidOperationException(name);"));

        meta.InsertStatement(switchBuilder.ToStatement());
        return 0;
    }

    [Template]
    private static void ReadType(IFieldOrProperty field, NetDataReader reader)
    {
        NetworkTemplates.ReadType(field.Type, reader, field);
    }

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    private void __UpdateReplicatedVar(int id, NetDataReader reader)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        var switchBuilder = new SwitchStatementBuilder(ExpressionFactory.Capture(id));
        var i = meta.CompileTime(0);
        foreach (var replicatedVar in replicatedVars)
        {
            var label = SwitchStatementLabel.CreateLiteral(i);
            switchBuilder.AddCase(label,
                StatementFactory.FromTemplate(nameof(ReadType),
                    new { field = replicatedVar, reader = ExpressionFactory.Capture(reader) }).UnwrapBlock());
            i++;
        }

        meta.InsertStatement(switchBuilder.ToStatement());
    }

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    private void __SerializeReplicatedVars(NetDataWriter writer)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        foreach (var replicatedVar in replicatedVars)
        {
            NetworkTemplates.WriteType(replicatedVar.Type, writer, replicatedVar.Value);
        }
    }

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    private void __DeserializeReplicatedVars(NetDataReader reader)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        foreach (var replicatedVar in replicatedVars)
        {
            NetworkTemplates.ReadType(replicatedVar.Type, reader, replicatedVar);
        }
    }
}