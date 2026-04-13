using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;
        var methodId = NetworkHelper.ComputeMethodId(meta.Target.Method);

        if (!networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            var writer = networkManager.ServerManager.GetRpcPacketData(entity.Id, methodId);

            foreach (var param in meta.Target.Method.Parameters)
            {
                writer.Put(param.Value);
            }

            networkManager.ClientManager.Send(writer);
            return null;
        }

        return meta.Proceed();
    }
}
