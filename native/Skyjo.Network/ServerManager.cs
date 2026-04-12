using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ServerManager : ManagerBase
{
    private const int FirstEntityId = 1;

    protected override string Role => "Server";
    private int _nextId = FirstEntityId;

    public event Action<NetPeer, NetDataReader>? OnPlayerConnected;

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
        var key = request.Data.GetString();
        if (key != Key)
        {
            Console.WriteLine($"[{Role}] Connection rejected (wrong key)");
            return;
        }

        var peer = request.Accept();
        Console.WriteLine($"[{Role}] Connection accepted");

        OnPlayerConnected?.Invoke(peer, request.Data);
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        Writer.Reset();
        foreach (var entity in Entities.Values)
        {
            var typeId = NetworkManager.GetEntityTypeId(entity.GetType());
            new EntityPacket(typeId, entity.Id, entity.OwnerId).Serialize(Writer);
        }

        peer.Send(Writer, DeliveryMethod.ReliableOrdered);
    }

    internal void Spawn(Entity entity)
    {
        entity.Id = _nextId++;
        entity.OwnerId = entity.Owner?.Id ?? -1;
        Entities[entity.Id] = entity;

        var typeId = NetworkManager.GetEntityTypeId(entity.GetType());

        Writer.Reset();
        new EntityPacket(typeId, entity.Id, entity.OwnerId).Serialize(Writer);
        NetManager.SendToAll(Writer, DeliveryMethod.ReliableOrdered);
    }
}