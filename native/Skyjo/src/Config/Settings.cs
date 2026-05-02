namespace Skyjo.Config;

public record Settings(
    string Username,
    NetMode NetMode,
    string Address,
    int Port,
    double TargetFramerate,
    Backend Backend,
    ViewRenderer ViewRenderer)
{
    public string Username { get; set; } = Username;
    public string Address { get; } = string.IsNullOrEmpty(Address) ? "127.0.0.1" : Address;
    public int Port { get; } = Port == 0 ? 1995 : Port;
    public double TargetFramerate { get; } = TargetFramerate == 0 ? 60 : TargetFramerate;
}