using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ClientManager : ManagerBase
{
    protected override string Role => "Client";
    public Action<NetDataWriter>? ConnectionData { get; set; }

    public ClientManager()
    {
        AddPacketHandler<CreateEntityPacket>(OnCreateEntityPacket);
        AddPacketHandler<DestroyEntityPacket>(OnDestroyEntityPacket);
        AddPacketHandler<ReplicatedPacket>(OnReplicatedPacket);
        AddPacketHandler<ReplicatedAllPacket>(_ => {});
    }

    public NetPeer Peer => NetManager.FirstPeer;
    public NetPeer? NullablePeer => IsRunning ? Peer : null;

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

    private void OnCreateEntityPacket(CreateEntityPacket packet)
    {
        var entity = NetworkManager.CreateEntity(packet.TypeId);
        entity.Id = packet.Id;
        entity.OwnerId = packet.OwnerId;
        NetworkManager.Entities[entity.Id] = entity;
    }

    private void OnDestroyEntityPacket(DestroyEntityPacket packet)
    {
        NetworkManager.Entities.Remove(packet.Id);
    }

    private void OnReplicatedPacket(ReplicatedPacket packet)
    {
        var entity = NetworkManager.Entities[packet.EntityId];
        entity.__UpdateReplicatedVar(packet.Index, packet.Reader);
    }
}