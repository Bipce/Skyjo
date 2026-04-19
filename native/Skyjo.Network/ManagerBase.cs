using System.ComponentModel;
using System.Net;
using System.Net.Sockets;
using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Attributes;
using Skyjo.Network.Packets;
using Skyjo.Network.Utils;

namespace Skyjo.Network;

public abstract class ManagerBase : INetEventListener
{
    private NetManager? _netManager;
    protected const string Key = "dd3722c44d3ac0b3919ba75bdd738654";

    private readonly Dictionary<Type, Action<Packet>> _packetHandlers = [];

    protected ManagerBase()
    {
        AddPacketHandler<RpcPacket>(OnRpcPacket);
    }

    protected NetManager NetManager
    {
        get => _netManager ?? throw new NullReferenceException(nameof(_netManager));
        private set => _netManager = value;
    }

    public int Port { get; set; } = 1995;
    public string Address { get; set; } = "127.0.0.1";

    protected abstract string Role { get; }
    public bool IsRunning => _netManager?.IsRunning == true;

    protected static NetworkManager NetworkManager => NetworkManager.Instance;
    protected static ServerManager ServerManager => NetworkManager.ServerManager;
    protected static ClientManager ClientManager => NetworkManager.ClientManager;

    public virtual bool Start()
    {
        if (IsRunning)
            return false;

        NetManager = new NetManager(this);
        return true;
    }

    public virtual void Update()
    {
        if (!IsRunning)
            return;

        NetManager.PollEvents();
    }

    public virtual bool Stop()
    {
        if (!IsRunning)
            return false;

        NetManager.Stop();
        _netManager = null;
        NetworkManager.Entities.Clear();

        Console.WriteLine($"[{Role}] Stopped");
        return true;
    }

    public virtual void OnPeerConnected(NetPeer peer)
    {
        Console.WriteLine($"[{Role}] Peer connected");
    }

    public virtual void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        Console.WriteLine($"[{Role}] Peer disconnected");
    }

    public void OnNetworkError(IPEndPoint endPoint, SocketError socketError)
    {
    }

    public void OnNetworkReceive(NetPeer peer, NetPacketReader reader, byte channelNumber,
        DeliveryMethod deliveryMethod)
    {
        while (reader.AvailableBytes > 0)
        {
            var packet = NetworkManager.CreatePacket(reader.GetByte());
            packet.Deserialize(reader);
            _packetHandlers[packet.GetType()].Invoke(packet);
        }

        reader.Recycle();
    }

    public void OnNetworkReceiveUnconnected(IPEndPoint remoteEndPoint, NetPacketReader reader,
        UnconnectedMessageType messageType)
    {
    }

    public void OnNetworkLatencyUpdate(NetPeer peer, int latency)
    {
    }

    public virtual void OnConnectionRequest(ConnectionRequest request)
    {
    }

    protected void AddPacketHandler<T>(Action<T> callback) where T : Packet
    {
        _packetHandlers[typeof(T)] = packet => callback((T)packet);
    }

    private void OnRpcPacket(RpcPacket packet)
    {
        var entity = NetworkManager.Entities[packet.EntityId];
#pragma warning disable CS0618
        entity.__CallMethod(packet.MethodId, packet.Reader);
#pragma warning restore CS0618
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    public void Send(byte channel = 0, DeliveryMethod deliveryMethod = DeliveryMethod.ReliableOrdered,
        NetPeer? excludePeer = null)
    {
        NetManager.SendToAll(NetworkManager.Writer, channel, deliveryMethod, excludePeer);
        NetworkManager.Writer.Reset();
    }
}