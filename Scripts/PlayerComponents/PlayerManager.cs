using System;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace DoubTech.Networking.PlayerComponents
{
    public class PlayerManager : NetworkSingelton<PlayerManager>
    {
        [SerializeField] private NetworkObject playerPrefab;

        [SerializeField] private PlayerSpawnPoint[] playerSpawnPoints;

        public UnityEvent<ulong> onClientConnected = new UnityEvent<ulong>();
        public UnityEvent<ulong> onClientDisconnected = new UnityEvent<ulong>();

        public UnityEvent onLocalClientConnected = new UnityEvent();

        public int PlayerCount => NetworkManager.Singleton.ConnectedClients.Count;

        private void Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;
        }

        protected virtual void OnClientDisconnected(ulong clientId)
        {
            onClientConnected.Invoke(clientId);
        }

        protected virtual void OnClientConnected(ulong clientId)
        {
            onClientDisconnected.Invoke(clientId);

            /*if (clientId == NetworkManager.Singleton.LocalClient.ClientId)
            {
                Debug.Log("Connected as client.");
                onLocalClientConnected.Invoke();
            }*/

            if (IsHost || IsServer)
            {
                var spawnPoint = playerSpawnPoints[0];
                for (int i = 0; i < playerSpawnPoints.Length; i++)
                {
                    if (playerSpawnPoints[i].IsAvailable)
                    {
                        spawnPoint = playerSpawnPoints[i];
                        break;
                    }
                }


                var prefab = Instantiate(playerPrefab);
                prefab.transform.position = spawnPoint.transform.position;
                prefab.transform.rotation = spawnPoint.transform.rotation;
                prefab.SpawnAsPlayerObject(clientId);
                //prefab.ChangeOwnership(clientId);
                Debug.Log($"Spawning {prefab.name} and changing ownership to {clientId}");
            }
        }
    }
}
