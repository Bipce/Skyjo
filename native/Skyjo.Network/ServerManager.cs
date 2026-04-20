using System.ComponentModel;
using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Packets;
using Skyjo.Network.Utils;

namespace Skyjo.Network;

public sealed class ServerManager : ManagerBase
{
    private const int FirstEntityId = 1;

    protected override string Role => "Server";
    private int _nextId = FirstEntityId;

    public event Action<NetPeer, NetDataReader>? OnPlayerConnected;
    public event Action? OnServerStarted;

    private readonly IndexedCollection<int, NetPeer> _peers = new(x => x.Id);
    private NetDataReader _lastReader = null!;

    private readonly IndexedCollection<int, ReplicatedFrequencyData> _frequencyData = new(x => x.NetUpdateFrequency);
    private readonly List<IReplicatedData> _invalidReplicatedData = [];

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

        request.Accept();
        Console.WriteLine($"[{Role}] Connection accepted");

        _lastReader = request.Data;
    }

    public override void OnPeerConnected(NetPeer peer)
    {
        base.OnPeerConnected(peer);

        if (peer.Id != ClientManager.NullablePeer?.Id)
        {
            _peers.Add(peer);

            new SendWorldPacket().Serialize(NetworkManager.Writer);
            peer.Send(NetworkManager.Writer, DeliveryMethod.ReliableOrdered);
            NetworkManager.Writer.Reset();
        }

        OnPlayerConnected?.Invoke(peer, _lastReader);
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        base.OnPeerDisconnected(peer, disconnectInfo);

        foreach (var entity in NetworkManager.Entities)
        {
            if (entity.Owner?.Id != peer.Id)
                continue;
            entity.IsPendingDestroy = true;
            NetworkManager.DestroyQueue.Enqueue(entity.Id);
            if (!HasRemotePeers)
                continue;
            new DestroyEntityPacket(entity.Id).Serialize(NetworkManager.Writer);
        }

        _peers.Remove(peer.Id);

        if (HasRemotePeers)
            SendToAll();
    }

    internal void Spawn(Entity entity)
    {
        entity.Id = _nextId++;
        entity.OwnerId = entity.Owner?.Id ?? -1;
        NetworkManager.SpawnQueue.Enqueue(entity);

        if (!HasRemotePeers)
            return;

        var typeId = NetworkManager.GetEntityTypeId(entity.GetType());
        new CreateEntityPacket(typeId, entity.Id, entity.OwnerId).Serialize(NetworkManager.Writer);
        entity.__SerializeReplicatedVars(NetworkManager.Writer);
        SendToAll();
    }

    internal void Destroy(Entity entity)
    {
        NetworkManager.DestroyQueue.Enqueue(entity.Id);

        if (!HasRemotePeers)
            return;

        new DestroyEntityPacket(entity.Id).Serialize(NetworkManager.Writer);
        SendToAll();
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public bool HasRemotePeers => _peers.Count > 0;

    public override void Update()
    {
        base.Update();

        if (!IsRunning)
            return;

        UpdateReplication();
    }

    private void UpdateReplication()
    {
        foreach (var frequencyData in _frequencyData)
        {
            frequencyData.Time += NetworkManager.DeltaTime;
            if (frequencyData.Time >= frequencyData.Frequency)
            {
                frequencyData.Time -= frequencyData.Frequency;

                foreach (var replicatedDataQueue in frequencyData.ReplicatedData)
                {
                    if (_invalidReplicatedData.Count > 0)
                    {
                        foreach (var data in _invalidReplicatedData)
                            replicatedDataQueue.Data.Enqueue(data);
                        _invalidReplicatedData.Clear();
                    }

                    while (replicatedDataQueue.Data.TryDequeue(out var data))
                    {
                        if (!data.Entity)
                            break;

                        if (!data.IsValid)
                        {
                            _invalidReplicatedData.Add(data);
                            continue;
                        }

                        if (!data.IsUnchanged)
                        {
                            new ReplicatedPacket(data.Entity.Id, data.Index).Serialize(NetworkManager.Writer);
                            data.Serialize(NetworkManager.Writer);
                        }

                        data.Done();
                    }

                    if (NetworkManager.Writer.Length > 0)
                    {
                        if (replicatedDataQueue.Peer != null)
                        {
                            replicatedDataQueue.Peer.Send(NetworkManager.Writer, replicatedDataQueue.Channel,
                                replicatedDataQueue.DeliveryMethod);
                        }
                        else
                        {
                            SendToAll(replicatedDataQueue.Channel, replicatedDataQueue.DeliveryMethod,
                                replicatedDataQueue.ExcludePeer);
                        }

                        NetworkManager.Writer.Reset();
                    }
                }
            }
        }
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public ReplicatedData<T> AddReplicatedData<T>(int netUpdateFrequency, byte channel, DeliveryMethod deliveryMethod,
        NetPeer? excludePeer, NetPeer? peer, Entity entity, int index, T lastValue, T value)
    {
        var data = new ReplicatedData<T>
        {
            Entity = entity,
            Index = index,
            LastValue = lastValue,
            Value = value
        };

        if (!_frequencyData.TryGetValue(netUpdateFrequency, out var frequencyData))
        {
            frequencyData = new ReplicatedFrequencyData(netUpdateFrequency);
            _frequencyData.Add(frequencyData);
        }

        var key = ReplicatedDataQueue.GetKey(channel, deliveryMethod, excludePeer, peer);
        if (!frequencyData.ReplicatedData.TryGetValue(key, out var replicatedDataQueue))
        {
            replicatedDataQueue = new ReplicatedDataQueue(key)
            {
                Channel = channel,
                DeliveryMethod = deliveryMethod,
                ExcludePeer = excludePeer,
                Peer = peer
            };
            frequencyData.ReplicatedData.Add(replicatedDataQueue);
        }

        replicatedDataQueue.Data.Enqueue(data);

        return data;
    }

    public void SendToAll(byte channel = 0, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered,
        NetPeer? excludePeer = null)
    {
        foreach (var peer in _peers)
        {
            if (excludePeer != null && peer.Id == excludePeer.Id)
                continue;

            peer.Send(NetworkManager.Writer, channel, deliveryMethod);
        }

        NetworkManager.Writer.Reset();
    }
}