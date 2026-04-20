using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using LiteNetLib;
using LiteNetLib.Utils;
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
    public int NetUpdateFrequency { get; set; } = 100;
    public bool IsPendingDestroy { get; internal set; }

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

    public void Destroy()
    {
        if (!ServerManager.IsRunning)
            throw new InvalidOperationException("Only the server can destroy entities");

        ServerManager.Destroy(this);
    }

    public bool Equals(Entity? other) => other is not null && Id == other.Id;
    public bool IsValid => NetworkManager.Entities.Contains(this);

    public static implicit operator bool([NotNullWhen(true)] Entity? entity) => entity is not null && entity.IsValid;

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void __CallMethod(int id, NetDataReader reader)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void __UpdateReplicatedVar(int id, NetDataReader reader)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void __SerializeReplicatedVars(NetDataWriter writer)
    {
    }

    [EditorBrowsable(EditorBrowsableState.Never)]
    protected internal virtual void __DeserializeReplicatedVars(NetDataReader reader)
    {
    }
}