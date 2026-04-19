namespace Skyjo.Network;

public record ReplicatedDataKey(byte Channel, int Reliability, int ExcludePeer, int Peer);