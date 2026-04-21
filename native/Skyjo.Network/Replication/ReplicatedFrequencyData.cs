using Skyjo.Network.Utils;

namespace Skyjo.Network.Replication;

public sealed class ReplicatedFrequencyData
{
    public ReplicatedFrequencyData(int netUpdateFrequency)
    {
        NetUpdateFrequency = netUpdateFrequency;
        Frequency = 1.0 / NetUpdateFrequency;
        Time = NetworkManager.Instance.TotalTime % Frequency;
    }

    public int NetUpdateFrequency { get; }
    public double Frequency { get; }

    public double Time { get; set; }

    public readonly IndexedCollection<ReplicatedDataKey, ReplicatedDataQueue> ReplicatedData = new(x => x.Key);
}