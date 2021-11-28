using System;
using UnityEngine;

namespace DoubTech.Networking.PlayerComponents
{
    [RequireComponent(typeof(Collider))]
    public class PlayerSpawnPoint : MonoBehaviour
    {
        private bool isAvailable = true;

        public bool IsAvailable => isAvailable;

        private void OnTriggerEnter(Collider other)
        {
            isAvailable = false;
        }

        private void OnTriggerExit(Collider other)
        {
            isAvailable = true;
        }
    }
}
