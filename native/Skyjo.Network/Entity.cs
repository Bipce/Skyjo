using LiteNetLib;

namespace Skyjo.Network;

public abstract class Entity
{
    private static NetworkManager NetworkManager => NetworkManager.Instance;
    private static ServerManager ServerManager => NetworkManager.ServerManager;
    private static ClientManager ClientManager => NetworkManager.ClientManager;

    public int Id { get; internal set; }
    public NetPeer? Owner { get; init; }
    internal int OwnerId { get; set; }

    public bool IsOwner
    {
        get
        {
            if (OwnerId == -1 || !ClientManager.IsRunning)
                return false;

            return OwnerId == ClientManager.Peer.RemoteId;
        }
    }

    public void Spawn()
    {
        if (!ServerManager.IsRunning)
            throw new InvalidOperationException("Only the server can spawn entities");

        ServerManager.Spawn(this);
    }
}