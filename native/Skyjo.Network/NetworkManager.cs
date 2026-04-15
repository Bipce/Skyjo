using LiteNetLib.Utils;
using Skyjo.Network.Attributes;
using Skyjo.Network.Enums;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class NetworkManager
{
    public static NetworkManager Instance { get; private set; } = null!;

    public ServerManager ServerManager { get; }
    public ClientManager ClientManager { get; }
    public bool IsRunning => ServerManager.IsRunning || ClientManager.IsRunning;

    private readonly Dictionary<byte, Func<Packet>> _packetFactories = [];
    private readonly Dictionary<Type, byte> _entityTypeIds = [];
    private readonly Dictionary<byte, Func<Entity>> _entityFactories = [];

    internal Dictionary<int, Entity> Entities { get; } = [];
    internal NetDataWriter Writer { get; private set; } = new();

    public double DeltaTimeMs { get; private set; }
    public double TotalTimeMs { get; private set; }

    public NetworkManager()
    {
        Instance = this;

        ServerManager = new ServerManager();
        ClientManager = new ClientManager();

        _packetFactories.Add((byte)PacketType.CreateEntity, () => new CreateEntityPacket());
        _packetFactories.Add((byte)PacketType.Rpc, () => new RpcPacket());
        _packetFactories.Add((byte)PacketType.DestroyEntity, () => new DestroyEntityPacket());
        _packetFactories.Add((byte)PacketType.Replicated, () => new ReplicatedPacket());
    }

    public void Update(double deltaTimeMs, double totalTimeMs)
    {
        DeltaTimeMs = deltaTimeMs;
        TotalTimeMs = totalTimeMs;

        ServerManager.Update();
        ClientManager.Update();
    }

    public void Stop()
    {
        ServerManager.Stop();
        ClientManager.Stop();
    }

    public IEnumerable<T> GetEntities<T>() where T : Entity
    {
        return Entities.Values.OfType<T>().OrderBy(e => e.Id);
    }

    public Entity GetEntity(int id)
    {
        return Entities[id];
    }

    public T GetEntity<T>(int id) where T : Entity
    {
        return (T)Entities[id];
    }

    internal Packet CreatePacket(byte id)
    {
        return _packetFactories[id].Invoke();
    }

    public void RegisterEntity<T>() where T : Entity, new()
    {
        var typeId = (byte)_entityTypeIds.Count;

        _entityTypeIds.Add(typeof(T), typeId);
        _entityFactories.Add(typeId, () => new T());
    }

    internal byte GetEntityTypeId(Type type)
    {
        return _entityTypeIds[type];
    }

    internal Entity CreateEntity(byte typeId)
    {
        return _entityFactories[typeId].Invoke();
    }

    [NetworkInternal]
    public NetDataWriter GetRpcPacketData(int entityId, int methodId)
    {
        Writer.Reset();
        new RpcPacket(entityId, methodId).Serialize(Writer);
        return Writer;
    }
}