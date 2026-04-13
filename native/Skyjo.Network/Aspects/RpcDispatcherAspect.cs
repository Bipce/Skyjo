using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Metalama.Framework.Code.SyntaxBuilders;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

internal sealed class RpcDispatcherAspect : TypeAspect
{
    public override void BuildAspect(IAspectBuilder<INamedType> builder)
    {
        var cases = builder.Target.Methods
            .Where(m => m.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect))))
            .Select(m => (
                Id: NetworkHelper.ComputeMethodId(m),
                Method: m,
                ReaderExpressions: m.Parameters
                    .Select(p => NetworkHelper.GetReaderExpression(p.Type))
                    .ToList()
            ))
            .ToList();

        builder.IntroduceMethod(
            nameof(InternalCallMethodTemplate),
            IntroductionScope.Instance,
            OverrideStrategy.Override,
            b =>
            {
                b.Name = "InternalCallMethod";
                b.Accessibility = Accessibility.Protected;
            },
            args: new { cases });
    }

    [Template]
    internal void InternalCallMethodTemplate(
        int id,
        NetDataReader reader,
        [CompileTime] List<(int Id, IMethod Method, List<string> ReaderExpressions)> cases)
    {
        foreach (var c in meta.CompileTime(cases))
        {
            if (id == c.Id)
            {
                var args = new List<IExpression>();
                foreach (var expr in meta.CompileTime(c.ReaderExpressions))
                {
                    args.Add(ExpressionFactory.Parse(expr));
                }
                c.Method.WithObject((IExpression)meta.This).Invoke(args);
                return;
            }
        }
    }
}
