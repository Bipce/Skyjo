using LiteNetLib;
using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;
using Skyjo.Network.Utils;

namespace Skyjo.Network.Attributes;

public sealed class ClientAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;
        var methodId = NetworkHelper.ComputeMethodId(meta.Target.Method);

        if (networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            if (entity.Owner == null)
                return null;

            if (networkManager.ServerManager.HasRemotePeers(out _))
            {
                var writer = networkManager.GetRpcPacketData(entity.Id, methodId);
                WriteParams(writer);
                entity.Owner.Send(writer, DeliveryMethod.ReliableOrdered);
                return null;
            }
        }

        return meta.Proceed();
    }
}