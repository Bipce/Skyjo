using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Attributes;

namespace Skyjo.Network;

public abstract class Entity
{
    protected NetworkManager NetworkManager => NetworkManager.Instance;
    protected ServerManager ServerManager => NetworkManager.ServerManager;
    protected ClientManager ClientManager => NetworkManager.ClientManager;

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

    [NetworkInternal]
    protected internal virtual void InternalCallMethod(int id, NetDataReader reader)
    {
    }
}