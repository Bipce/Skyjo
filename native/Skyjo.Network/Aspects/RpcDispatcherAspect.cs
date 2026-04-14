using LiteNetLib.Utils;
using Metalama.Framework.Aspects;
using Metalama.Framework.Code;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Aspects;

internal sealed class RpcDispatcherAspect : TypeAspect
{
    [Introduce] private static Dictionary<int, Action<NetDataReader>>? _rpcHandlers;

    [Introduce(Accessibility = Accessibility.Protected, WhenExists = OverrideStrategy.Override)]
    private void InternalCallMethod(int id, NetDataReader reader)
    {
        var methods = meta.Target.Type.Methods
            .Where(x => x.Attributes.Any(a => a.Type.IsConvertibleTo(typeof(RpcMethodAspect)))).ToArray();

        if (_rpcHandlers == null)
        {
            _rpcHandlers = new Dictionary<int, Action<NetDataReader>>(methods.Length);
            foreach (var method in methods)
            {
                var methodId = NetworkHelper.ComputeMethodId(method);
                var parameters = method.Parameters.Select(x => NetworkHelper.GetReaderExpression(x.Type));
                var args = string.Join(", ", parameters);
                var body = $"this.{method.Name}({args});";
                meta.InsertStatement($"_rpcHandlers[{methodId}] = (reader) => {{{body}}};");
            }
        }

        _rpcHandlers[id](reader);
    }
}