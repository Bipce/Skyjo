using LiteNetLib;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ClientManager : ManagerBase
{
    protected override string Role => "Client";

    public ClientManager()
    {
        AddPacketHandler<EntityPacket>(OnEntityPacket);
    }

    public override bool Start()
    {
        if (!base.Start())
            return false;
        var state = NetManager.Start();
        if (!state)
            return false;
        NetManager.Connect(Address, Port, Key);
        Console.WriteLine($"[{Role}] Connecting");
        return true;
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        base.OnPeerDisconnected(peer, disconnectInfo);
        Stop();
    }

    private void OnEntityPacket(EntityPacket packet)
    {
        var entity = NetworkManager.CreateEntity(packet.TypeId);
        entity.Id = packet.Id;
        Entities[entity.Id] = entity;
    }
}