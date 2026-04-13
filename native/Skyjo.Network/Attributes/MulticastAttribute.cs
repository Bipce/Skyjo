using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MulticastAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;
        var methodId = NetworkHelper.ComputeMethodId(meta.Target.Method);

        if (networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            var writer = networkManager.ServerManager.GetRpcPacketData(entity.Id, methodId);
            WriteParams(writer);
            networkManager.ServerManager.Send(writer);
            return null;
        }

        return meta.Proceed();
    }
}