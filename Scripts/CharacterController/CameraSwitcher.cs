using Cinemachine;
using UnityEngine;

namespace StarterAssets
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera cameraA;
        [SerializeField] private CinemachineVirtualCamera cameraB;

        public void Swap()
        {
            int a = cameraA.Priority;
            int b = cameraB.Priority;
            cameraA.Priority = b;
            cameraB.Priority = a;
        }
    }
}
