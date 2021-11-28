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

                var prefab = Instantiate(playerPrefab);
                prefab.transform.position = spawnPoint.transform.position;
                prefab.transform.rotation = spawnPoint.transform.rotation;
                prefab.SpawnAsPlayerObject(clientId);
            }
        }
    }
}
