using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Attributes;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ClientManager : ManagerBase
{
    protected override string Role => "Client";
    public Action<NetDataWriter>? ConnectionData { get; set; }

    public ClientManager()
    {
        AddPacketHandler<EntityPacket>(OnEntityPacket);
    }

    public NetPeer Peer => NetManager.FirstPeer;

    public override bool Start()
    {
        if (!base.Start())
            return false;
        var state = NetManager.Start();
        if (!state)
            return false;

        var writer = new NetDataWriter();
        writer.Put(Key);
        ConnectionData?.Invoke(writer);
        NetManager.Connect(Address, Port, writer);
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
        entity.OwnerId = packet.OwnerId;
        Entities[entity.Id] = entity;
    }

    [NetworkInternal]
    public void Send(NetDataWriter writer)
    {
        NetManager.SendToAll(writer, DeliveryMethod.ReliableOrdered);
    }
}