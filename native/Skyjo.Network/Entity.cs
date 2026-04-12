using LiteNetLib;

namespace Skyjo.Network;

public abstract class Entity
{
    private static NetworkManager NetworkManager => NetworkManager.Instance;
    private static ServerManager ServerManager => NetworkManager.ServerManager;
    private static ClientManager ClientManager => NetworkManager.ClientManager;

    public int Id { get; internal set; }
    public NetPeer? Owner { get; init; }

    public void Spawn()
    {
        if (!ServerManager.IsRunning)
            throw new InvalidOperationException("Only the server can spawn entities");

        ServerManager.Spawn(this);
    }
}