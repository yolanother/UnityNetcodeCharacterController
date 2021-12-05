using System.Collections;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

namespace DoubTech.Networking.PlayerComponents
{
    public class PlayerManager : NetworkSingelton<PlayerManager>
    {
        [SerializeField] private NetworkObject playerPrefab;
        [SerializeField] private PlayerSpawnPoint[] playerSpawnPoints;
        [SerializeField] private bool scanSceneForAdditionalSpawnPoints;

        public UnityEvent<ulong> onClientConnected = new UnityEvent<ulong>();
        public UnityEvent<ulong> onClientDisconnected = new UnityEvent<ulong>();

        public UnityEvent onLocalClientConnected = new UnityEvent();

        public int PlayerCount => NetworkManager.Singleton.ConnectedClients.Count;

        private List<PlayerSpawnPoint> sceneSpawnPoints = new List<PlayerSpawnPoint>();
        private HashSet<ulong> processedClients = new HashSet<ulong>();

        private void OnEnable()
        {
            if (null != playerSpawnPoints)
            {
                sceneSpawnPoints.AddRange(playerSpawnPoints);
            }

            if (scanSceneForAdditionalSpawnPoints)
            {
                sceneSpawnPoints.AddRange(FindObjectsOfType<PlayerSpawnPoint>());
            }
        }

        private IEnumerator Start()
        {
            NetworkManager.Singleton.OnClientConnectedCallback += OnClientConnected;
            NetworkManager.Singleton.OnClientDisconnectCallback += OnClientDisconnected;

            Debug.Log("Player manager waiting for connection...");

            yield return new WaitUntil(() => NetworkManager.Singleton.IsConnectedClient || NetworkManager.IsHost);
            Debug.Log("Player is connected.");
            if (NetworkManager.Singleton.IsConnectedClient || NetworkManager.IsHost)
            {
                OnClientConnected(NetworkManager.Singleton.LocalClientId);
            }
        }

        protected virtual void OnClientDisconnected(ulong clientId)
        {
            onClientConnected.Invoke(clientId);
            processedClients.Remove(clientId);
        }

        protected virtual void OnClientConnected(ulong clientId)
        {
            if (processedClients.Contains(clientId)) return;
            processedClients.Add(clientId);
            onClientConnected.Invoke(clientId);

            if (IsHost || IsServer)
            {
                var spawnPoint = sceneSpawnPoints[0];
                for (int i = 0; i < sceneSpawnPoints.Count; i++)
                {
                    if (sceneSpawnPoints[i].IsAvailable)
                    {
                        spawnPoint = sceneSpawnPoints[i];
                        break;
                    }
                }

                var prefab = Instantiate(playerPrefab, spawnPoint.transform.position, spawnPoint.transform.rotation);
                prefab.SpawnAsPlayerObject(clientId);
            }
        }
    }
}
