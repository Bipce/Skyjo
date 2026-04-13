using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

internal sealed class RpcDispatcherAspect : TypeAspect
{
    [Template] private static Dictionary<int, Action<Entity, NetDataReader>>? _rpcHandlers;

    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        var cases = builder.Target.Methods
            .Where(m => m.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))))
            .Select(m =>
            {
                var args = string.Join(", ", m.Parameters
                    .Select(p => NetworkHelper.GetReaderExpression(p.Type).Replace("reader.", "r.")));
                var lambda = $"(Skyjo.Network.Entity e, LiteNetLib.Utils.NetDataReader r) => " +
                             $"(({m.DeclaringType.FullName})e).{m.Name}({args})";
                return (Id: NetworkHelper.ComputeMethodId(m), LambdaString: lambda);
            })
            .ToList();

        var fieldResult =
            builder.IntroduceField(nameof(_rpcHandlers), IntroductionScope.Static, OverrideStrategy.Ignore);

        builder.IntroduceMethod(
            nameof(InternalCallMethodTemplate),
            IntroductionScope.Instance,
            OverrideStrategy.Override,
            b =>
            {
                b.Name = "InternalCallMethod";
                b.Accessibility = Accessibility.Protected;
            },
            args: new { cases, handlersField = fieldResult.Declaration });
    }

    [Template]
    internal void InternalCallMethodTemplate(
        int id,
        NetDataReader reader,
        [CompileTime] List<(int Id, string LambdaString)> cases,
        [CompileTime] IField handlersField)
    {
        if (handlersField.Value == null)
        {
            handlersField.Value = ExpressionFactory.Parse(
                meta.CompileTime(
                    "new System.Collections.Generic.Dictionary<int, System.Action<Skyjo.Network.Entity, LiteNetLib.Utils.NetDataReader>>(" +
                    cases.Count + ") { " +
                    string.Join(", ", cases.Select(c => $"[{c.Id}] = {c.LambdaString}")) +
                    " }"));
        }

        ((Dictionary<int, Action<Entity, NetDataReader>>)handlersField.Value!)[id]((Entity)meta.This, reader);
    }
}