namespace Skyjo.Network.Replication;

public record ReplicatedDataKey(byte Channel, int Reliability, int ExcludePeer, int Peer);