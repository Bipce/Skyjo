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
                ReaderMethodNames: m.Parameters
                    .Select(p => NetworkHelper.GetReaderGetMethod(p.Type))
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
        [CompileTime] List<(int Id, IMethod Method, List<string> ReaderMethodNames)> cases)
    {
        foreach (var c in meta.CompileTime(cases))
        {
            if (id == c.Id)
            {
                var args = new List<IExpression>();
                foreach (var methodName in meta.CompileTime(c.ReaderMethodNames))
                {
                    args.Add(ExpressionFactory.Parse($"reader.{methodName}()"));
                }
                c.Method.WithObject((IExpression)meta.This).Invoke(args);
                return;
            }
        }
    }
}
