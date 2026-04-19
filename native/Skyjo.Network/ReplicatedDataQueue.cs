using LiteNetLib;

namespace Skyjo.Network;

public sealed class ReplicatedDataQueue
{
    public ReplicatedDataQueue(ReplicatedDataKey key)
    {
        Key = key;
    }

    public required byte Channel { get; init; }
    public required DeliveryMethod DeliveryMethod { get; init; }
    public required NetPeer? ExcludePeer { get; init; }
    public required NetPeer? Peer { get; init; }
    public Queue<IReplicatedData> Data { get; init; } = [];

    public ReplicatedDataKey Key { get; }

    public static ReplicatedDataKey GetKey(byte channel, DeliveryMethod deliveryMethod, NetPeer? excludePeer,
        NetPeer? peer)
    {
        return new ReplicatedDataKey(channel, (int)deliveryMethod, excludePeer?.Id ?? -1, peer?.Id ?? -1);
    }
}