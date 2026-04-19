using LiteNetLib;
using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;

namespace Skyjo.Network.Attributes;

[AttributeUsage(AttributeTargets.Method)]
public sealed class MulticastAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;

        if (networkManager.ServerManager is { IsRunning: true, HasRemotePeers: true })
        {
            var entity = (Entity)meta.This;
            var writer = networkManager.GetRpcPacketData(entity.Id, GetMethodId());
            WriteParams(writer);
            networkManager.ServerManager.SendToAll(Channel, (DeliveryMethod)meta.RunTime((int)Reliability));
        }

        return meta.Proceed();
    }
}