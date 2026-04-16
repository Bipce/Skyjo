using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Attributes;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class ServerManager : ManagerBase
{
    private const int FirstEntityId = 1;

    protected override string Role => "Server";
    private int _nextId = FirstEntityId;

    public event Action<NetPeer, NetDataReader>? OnPlayerConnected;
    public event Action? OnServerStarted;

    private readonly Dictionary<int, NetPeer> _peers = [];
    private NetDataReader _lastReader = null!;

    private readonly Dictionary<double, ReplicatedFrequencyData> _frequencyData = [];

    public override bool Start()
    {
        if (!base.Start() || ClientManager.IsRunning)
            return false;

        var state = NetManager.Start(Port);
        if (state)
        {
            Console.WriteLine($"[{Role}] Server listening on port {Port}");
            OnServerStarted?.Invoke();
        }

        return state;
    }

    public override bool Stop()
    {
        if (!base.Stop())
            return false;

        _nextId = FirstEntityId;
        _peers.Clear();
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
        _peers.Add(peer.Id, peer);

        _lastReader = request.Data;
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        if (peer.Id != ClientManager.NullablePeer?.Id)
        {
            foreach (var entity in NetworkManager.Entities.Values)
            {
                var typeId = NetworkManager.GetEntityTypeId(entity.GetType());
                new CreateEntityPacket(typeId, entity.Id, entity.OwnerId).Serialize(NetworkManager.Writer);
            }

            peer.Send(NetworkManager.Writer, DeliveryMethod.ReliableOrdered);
            NetworkManager.Writer.Reset();
        }

        OnPlayerConnected?.Invoke(peer, _lastReader);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        base.OnPeerDisconnected(peer, disconnectInfo);

        foreach (var entity in NetworkManager.Entities.Values)
        {
            if (entity.Owner?.Id != peer.Id)
                continue;
            new DestroyEntityPacket(entity.Id).Serialize(NetworkManager.Writer);
            NetworkManager.Entities.Remove(entity.Id);
        }

        _peers.Remove(peer.Id);

        Send();
    }

    internal void Spawn(Entity entity)
    {
        entity.Id = _nextId++;
        entity.OwnerId = entity.Owner?.Id ?? -1;
        NetworkManager.Entities[entity.Id] = entity;

        if (!HasRemotePeers(out var excludePeer))
            return;

        var typeId = NetworkManager.GetEntityTypeId(entity.GetType());
        NetworkManager.Writer.Reset();
        new CreateEntityPacket(typeId, entity.Id, entity.OwnerId).Serialize(NetworkManager.Writer);
        Send(excludePeer: excludePeer);
    }

    [NetworkInternal]
    public bool HasRemotePeers(out NetPeer? peer)
    {
        switch (NetManager.ConnectedPeersCount)
        {
            case 0:
            case 1 when ClientManager.IsRunning:
                peer = null;
                return false;
        }

        peer = _peers.GetValueOrDefault(ClientManager.NullablePeer?.Id ?? -1);

        return true;
    }

    public override void Update()
    {
        base.Update();

        if (!IsRunning)
            return;

        UpdateReplication();
    }

    private void UpdateReplication()
    {
        foreach (var (frequency, frequencyData) in _frequencyData)
        {
            frequencyData.Time += NetworkManager.DeltaTimeMs;
            if (frequencyData.Time >= frequency)
            {
                frequencyData.Time -= frequency;

                while (frequencyData.ReplicatedData.TryDequeue(out var data))
                {
                    if (!data.IsUnchanged)
                    {
                        new ReplicatedPacket(data.Entity.Id, data.Index).Serialize(NetworkManager.Writer);
                        data.Serialize(NetworkManager.Writer);
                    }

                    data.Done();
                }

                _frequencyData.Remove(frequency);
            }
        }

        if (NetworkManager.Writer.Length > 0 && HasRemotePeers(out var excludePeer))
        {
            Send(excludePeer: excludePeer);
        }
    }

    [NetworkInternal]
    public ReplicatedData<T> AddReplicatedData<T>(double frequency, Entity entity, int index, T lastValue, T value)
    {
        var data = new ReplicatedData<T>
        {
            Entity = entity,
            Index = index,
            LastValue = lastValue,
            Value = value
        };

        if (!_frequencyData.TryGetValue(frequency, out var frequencyData))
        {
            frequencyData = new ReplicatedFrequencyData(frequency);
            _frequencyData[frequency] = frequencyData;
        }

        frequencyData.ReplicatedData.Enqueue(data);

        return data;
    }
}