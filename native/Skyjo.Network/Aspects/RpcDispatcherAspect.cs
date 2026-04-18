using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

internal sealed class RpcDispatcherAspect : TypeAspect
{
    [Template]
    private static void ReadParameters(IMethod method, NetDataReader reader)
    {
        List<IExpression> args = [];

        foreach (var parameter in method.Parameters)
        {
            var field = meta.DefineLocalVariable("data", parameter.Type);
            NetworkTemplates.ReadType(parameter.Type, reader, field);
            args.Add(field);
        }

        method.Invoke(args);
    }

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    private void __CallMethod(int id, NetDataReader reader)
    {
        var methods = meta.Target.Type.Methods
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))));

        var switchBuilder = new SwitchStatementBuilder(ExpressionFactory.Capture(id));
        var i = meta.CompileTime(0);
        foreach (var method in methods)
        {
            var label = SwitchStatementLabel.CreateLiteral(i);

            switchBuilder.AddCase(label,
                StatementFactory.FromTemplate(nameof(ReadParameters),
                    new { method, reader = ExpressionFactory.Capture(reader) }).UnwrapBlock());
            i++;
        }

        meta.InsertStatement(switchBuilder.ToStatement());
    }
}