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

        if (networkManager.ServerManager.IsRunning && networkManager.ServerManager.HasRemotePeers(out var excludePeer))
        {
            var entity = (Entity)meta.This;
            var writer = networkManager.GetRpcPacketData(entity.Id, GetMethodId());
            WriteParams();
            networkManager.ServerManager.Send(writer, Channel, (DeliveryMethod)meta.RunTime((int)Reliability),
                excludePeer: excludePeer);
        }

        return meta.Proceed();
    }
}