using LiteNetLib;

namespace Skyjo.Network;

public sealed class ServerManager : ManagerBase
{
    protected override string Role => "Server";

    public override bool Start()
    {
        if (!base.Start() || NetworkManager.ClientManager.IsRunning)
            return false;

        var state = NetManager.Start(Port);
        if (state)
            Console.WriteLine($"[{Role}] Server listening on port {Port}");
        return state;
    }

    public override void OnConnectionRequest(ConnectionRequest request)
    {
        var peer = request.AcceptIfKey(Key);
        if (peer != null)
            Console.WriteLine($"[{Role}] Connection accepted");
        else
            Console.WriteLine($"[{Role}] Connection rejected (wrong key)");
    }
}