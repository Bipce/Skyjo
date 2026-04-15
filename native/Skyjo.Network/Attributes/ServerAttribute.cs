using LiteNetLib;
using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;

namespace Skyjo.Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class ServerAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;

        if (!networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            networkManager.GetRpcPacketData(entity.Id, GetMethodId());
            WriteParams();
            networkManager.ClientManager.Send(Channel, (DeliveryMethod)meta.RunTime((int)Reliability));
            return null;
        }

        return meta.Proceed();
    }
}