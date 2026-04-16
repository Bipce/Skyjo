using System.ComponentModel;
using LiteNetLib;
using LiteNetLib.Utils;
using Skyjo.Network.Attributes;
using Skyjo.Network.Utils;

namespace Skyjo.Network;

public abstract class Entity : IEquatable<Entity>
{
    protected NetworkManager NetworkManager => NetworkManager.Instance;
    protected ServerManager ServerManager => NetworkManager.ServerManager;
    protected ClientManager ClientManager => NetworkManager.ClientManager;

    public int Id { get; internal set; }
    public NetPeer? Owner { get; init; }
    internal int OwnerId { get; set; }
    public float NetUpdateFrequency { get; set; } = 100;

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

    public bool Equals(Entity? other) => other is not null && Id == other.Id;

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(NetworkHelper.InternalMessage)]
    protected internal virtual void __CallMethod(int id, NetDataReader reader)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    [Obsolete(NetworkHelper.InternalMessage)]
    protected internal virtual void __UpdateReplicatedVar(int id, NetDataReader reader)
    {
    }
}