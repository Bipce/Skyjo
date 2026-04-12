using LiteNetLib;

namespace Skyjo.Network;

public sealed class ClientManager : ManagerBase
{
    protected override string Role => "Client";

    public override bool Start()
    {
        if (!base.Start())
            return false;
        var state = NetManager.Start();
        if (!state)
            return false;
        NetManager.Connect(Address, Port, Key);
        Console.WriteLine($"[{Role}] Connecting");
        return true;
    }

    public override void OnPeerDisconnected(NetPeer peer, DisconnectInfo disconnectInfo)
    {
        base.OnPeerDisconnected(peer, disconnectInfo);
        Stop();
    }
}