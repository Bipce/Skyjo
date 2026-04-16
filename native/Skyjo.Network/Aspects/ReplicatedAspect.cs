using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Skyjo.Network.Attributes;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

public sealed class ReplicatedAspect : TypeAspect
{
    [Introduce(Accessibility = Accessibility.Private, WhenExists = OverrideStrategy.Ignore)]
    private static int GetReplicatedVarIndex(string name)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        var sb = meta.CompileTime(new System.Text.StringBuilder("return name switch { "));
        var i = meta.CompileTime(0);
        foreach (var replicatedVar in replicatedVars)
        {
            sb.Append($"\"{replicatedVar.Name}\" => {i}, ");
            i++;
        }

        sb.Append("_ => throw new global::System.InvalidOperationException(name) };");
        meta.InsertStatement(sb.ToString());
        return default;
    }

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    [NetworkInternal]
    private void InternalUpdateReplicatedVar(int id, NetDataReader reader)
    {
        var replicatedVars = meta.Target.Type.FieldsAndProperties
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(ReplicatedAttribute))));

        var sb = meta.CompileTime(new System.Text.StringBuilder("switch(id){"));
        var i = meta.CompileTime(0);
        foreach (var replicatedVar in replicatedVars)
        {
            var expr = NetworkHelper.GetReaderExpression(replicatedVar.Type);
            sb.Append($"case {i}: this.{replicatedVar.Name} = {expr}; return;");
            i++;
        }

        sb.Append('}');
        meta.InsertStatement(sb.ToString());
    }
}
