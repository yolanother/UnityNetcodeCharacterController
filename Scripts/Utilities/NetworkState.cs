using DoubTech.Networking;
using Unity.Netcode;
using UnityEngine;

namespace DoubTech.Multiplayer
{
    public class NetworkState : NetworkSingelton<NetworkState>
    {
        [SerializeField] private bool playOffline;
        public bool IsConnected => playOffline || NetworkManager.IsConnectedClient;
        public bool IsOwner(NetworkObject obj) => playOffline || obj.IsOwner;
    }
}
