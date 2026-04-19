using LiteNetLib;
using Metalama.Framework.Aspects;
using Skyjo.Network.Aspects;

namespace Skyjo.Network.Attributes;

public sealed class ClientAttribute : RpcMethodAspect
{
    public override dynamic? OverrideMethod()
    {
        var networkManager = NetworkManager.Instance;

        if (networkManager.ServerManager.IsRunning)
        {
            var entity = (Entity)meta.This;
            if (entity.Owner == null)
                return null;

            if (networkManager.ServerManager.HasRemotePeers)
            {
                var writer = networkManager.GetRpcPacketData(entity.Id, GetMethodId());
                WriteParams(writer);
                entity.Owner.Send(writer, Channel, (DeliveryMethod)meta.RunTime((int)Reliability));
                writer.Reset();
                return null;
            }
        }

        return meta.Proceed();
    }
}