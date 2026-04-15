using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Skyjo.Network.Attributes;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

internal sealed class RpcDispatcherAspect : TypeAspect
{
    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    [NetworkInternal]
    private void InternalCallMethod(int id, NetDataReader reader)
    {
        var methods = meta.Target.Type.Methods
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))));

        var sb = meta.CompileTime(new System.Text.StringBuilder("switch(id){"));
        var i = meta.CompileTime(0);
        foreach (var method in methods)
        {
            var parameters = method.Parameters.Select(x => NetworkHelper.GetReaderExpression(x.Type));
            var args = string.Join(", ", parameters);
            sb.Append($"case {i}: this.{method.Name}({args}); return;");
            i++;
        }

        sb.Append('}');
        meta.InsertStatement(sb.ToString());
    }
}