using LiteNetLib;

namespace Skyjo.Network.Extensions;

public static class PeerExtensions
{
    extension(NetPeer peer)
    {
        public int ServerId
        {
            get
            {
                var networkManager = NetworkManager.Instance;
                return networkManager.ServerManager.IsRunning ? peer.Id : peer.RemoteId;
            }
        }
    }
}