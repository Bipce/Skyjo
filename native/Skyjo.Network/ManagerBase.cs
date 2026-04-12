using System.Net;
using System.Net.Sockets;
using LiteNetLib;

namespace Skyjo.Network;

public abstract class ManagerBase : INetEventListener
{
    private NetManager? _netManager;
    protected const string Key = "dd3722c44d3ac0b3919ba75bdd738654";

    protected NetManager NetManager
    {
        get => _netManager ?? throw new NullReferenceException(nameof(_netManager));
        set => _netManager = value;
    }

    public int Port { get; set; } = 1995;
    public string Address { get; set; } = "127.0.0.1";

    protected abstract string Role { get; }
    public bool IsRunning => _netManager?.IsRunning == true;

    protected NetworkManager NetworkManager => NetworkManager.Instance;

    public virtual bool Start()
    {
        if (IsRunning)
            return false;

        NetManager = new NetManager(this);
        return true;
    }

    public void Update()
    {
        if (!IsRunning)
            return;

        NetManager.PollEvents();
    }

    public void Stop()
    {
        if (!IsRunning)
            return;

        NetManager.Stop();
        _netManager = null;

        Console.WriteLine($"[{Role}] Stopped");
    }

    public void OnPeerConnected(NetPeer peer)
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
}