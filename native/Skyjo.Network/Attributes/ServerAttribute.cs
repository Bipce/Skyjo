using LiteNetLib;
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
            var writer = networkManager.GetRpcPacketData(entity.Id, methodId);
            WriteParams(writer);
            networkManager.ClientManager.Send(writer, Channel, (DeliveryMethod)meta.RunTime((int)Reliability));
            return null;
        }

        return meta.Proceed();
    }
}
