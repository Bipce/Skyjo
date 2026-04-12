using LiteNetLib;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ServerManager : ManagerBase
{
    private const int FirstEntityId = 1;

    protected override string Role => "Server";
    private int _nextId = FirstEntityId;

    public override bool Start()
    {
        if (!base.Start() || ClientManager.IsRunning)
            return false;

        var state = NetManager.Start(Port);
        if (state)
            Console.WriteLine($"[{Role}] Server listening on port {Port}");
        return state;
    }

    public override bool Stop()
    {
        if (!base.Stop())
            return false;

        _nextId = FirstEntityId;
        return true;
    }

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        var peer = request.AcceptIfKey(Key);
        if (peer == null)
        {
            Console.WriteLine($"[{Role}] Connection rejected (wrong key)");
            return;
        }

        Console.WriteLine($"[{Role}] Connection accepted");
        // todo: create entity player
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        Writer.Reset();
        foreach (var entity in Entities.Values)
        {
            var typeId = NetworkManager.GetEntityTypeId(entity.GetType());
            var packet = new EntityPacket(typeId, entity.Id);
            Writer.Put((byte)packet.Type);
            packet.Serialize(Writer);
        }

        peer.Send(Writer, DeliveryMethod.ReliableOrdered);
    }

    public void Spawn<T>(T entity) where T : Entity
    {
        entity.Id = _nextId++;
        Entities[entity.Id] = entity;

        var typeId = NetworkManager.GetEntityTypeId(typeof(T));
        var packet = new EntityPacket(typeId, entity.Id);
        SendToAll(packet);
    }

    private void SendToAll<T>(T packet) where T : Packet
    {
        Writer.Reset();
        Writer.Put((byte)packet.Type);
        packet.Serialize(Writer);
        NetManager.SendToAll(Writer, DeliveryMethod.ReliableOrdered);
    }
}