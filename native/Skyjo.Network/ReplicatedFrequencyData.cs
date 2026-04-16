namespace Skyjo.Network;

public sealed class ReplicatedFrequencyData
{
    public ReplicatedFrequencyData(double frequency)
    {
        Time = NetworkManager.Instance.TotalTimeMs % frequency;
    }

    public double Time { get; set; }
    public readonly Queue<IReplicatedData> ReplicatedData = [];
}