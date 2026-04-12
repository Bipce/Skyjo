namespace Skyjo.Network;

public sealed class NetworkManager
{
    public static NetworkManager Instance { get; private set; } = null!;

    public ServerManager ServerManager { get; }
    public ClientManager ClientManager { get; }

    public NetworkManager()
    {
        Instance = this;

        ServerManager = new ServerManager();
        ClientManager = new ClientManager();
    }

    public void Update()
    {
        ServerManager.Update();
        ClientManager.Update();
    }

    public void Stop()
    {
        ServerManager.Stop();
        ClientManager.Stop();
    }
}