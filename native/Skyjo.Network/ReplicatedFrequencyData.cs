namespace Skyjo.Network;

public sealed class ReplicatedFrequencyData
{
    public ReplicatedFrequencyData(double frequency)
    {
        Frequency = frequency;
        Time = NetworkManager.Instance.TotalTime % frequency;
    }

    public double Frequency { get; }
    public double Time { get; set; }
    public readonly Queue<IReplicatedData> ReplicatedData = [];
}