using Skyjo.Network.Enums;
using Skyjo.Network.Packets;

namespace Skyjo.Network;

public sealed class NetworkManager
{
    public static NetworkManager Instance { get; private set; } = null!;

    public ServerManager ServerManager { get; }
    public ClientManager ClientManager { get; }

    private readonly Dictionary<byte, Func<Packet>> _packetFactories = [];
    private readonly Dictionary<Type, byte> _entityTypeIds = [];
    private readonly Dictionary<byte, Func<Entity>> _entityFactories = [];

    public NetworkManager()
    {
        Instance = this;

        ServerManager = new ServerManager();
        ClientManager = new ClientManager();

        _packetFactories.Add((byte)PacketType.Entity, () => new EntityPacket());
    }

    public void Update()
    {
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
        var data = ServerManager.IsRunning ? ServerManager.Entities : ClientManager.Entities;
        return data.Values.OfType<T>().OrderBy(e => e.Id);
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
}