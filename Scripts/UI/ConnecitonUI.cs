using Unity.Netcode;
using UnityEngine;
using UnityEngine.UI;

namespace DoubTech.Networking
{
    public class ConnecitonUI : MonoBehaviour
    {
        [SerializeField] private Button serverButton;
        [SerializeField] private Button hostButton;
        [SerializeField] private Button clientButton;
        [SerializeField] private Button disconnectButton;

        private bool initialized = false;

        private void Awake()
        {
            disconnectButton.gameObject.SetActive(false);
            serverButton.onClick.AddListener(() =>
            {
                Initialize();
                NetworkManager.Singleton.StartServer();
            });
            hostButton.onClick.AddListener(() =>
            {
                Initialize();
                NetworkManager.Singleton.StartHost();
            });
            clientButton.onClick.AddListener(() => {
                Initialize();
                NetworkManager.Singleton.StartClient();
            });
            disconnectButton.onClick.AddListener(() =>
            {
                Initialize();
                NetworkManager.Singleton.Shutdown();
                OnDisconnected();
            });
        }

        private void Initialize()
        {
            if (initialized) return;
            initialized = true;

            NetworkManager.Singleton.OnServerStarted += OnConnected;
            NetworkManager.Singleton.OnClientConnectedCallback += (uid) =>
            {
                if (uid == NetworkManager.Singleton.LocalClientId) OnConnected();
            };
            NetworkManager.Singleton.OnClientDisconnectCallback += (uid) =>
            {
                if (uid == NetworkManager.Singleton.LocalClientId) OnDisconnected();
            };
        }

        private void OnDisconnected()
        {
            serverButton.gameObject.SetActive(true);
            hostButton.gameObject.SetActive(true);
            clientButton.gameObject.SetActive(true);
            disconnectButton.gameObject.SetActive(false);
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        public void OnConnected()
        {
            serverButton.gameObject.SetActive(false);
            hostButton.gameObject.SetActive(false);
            clientButton.gameObject.SetActive(false);
            disconnectButton.gameObject.SetActive(true);
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }
    }
}
