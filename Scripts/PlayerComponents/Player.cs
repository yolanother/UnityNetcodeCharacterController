using System;
using Cinemachine;
using DoubTech.Multiplayer;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Events;

public class Player : NetworkBehaviour
{
    [SerializeField] private Transform fpsFollowTarget;
    [SerializeField] private Transform tpsFollowTarget;

    [SerializeField] private Behaviour[] localPlayerBehaviours;
    [SerializeField] private Behaviour[] remotePlayerBehaviours;

    [SerializeField] public UnityEvent onSwitchedToFPS = new UnityEvent();
    [SerializeField] public UnityEvent onSwitchedToTPS = new UnityEvent();

    public bool IsFPS => CinemachineCore.Instance?.GetActiveBrain(0)?.ActiveVirtualCamera
        ?.VirtualCameraGameObject?.CompareTag("FPSVirtualCamera") ?? false;

    private void OnEnable()
    {
        if (IsFPS) onSwitchedToFPS.Invoke();
        else onSwitchedToTPS.Invoke();
        CinemachineCore.Instance.GetActiveBrain(0).m_CameraActivatedEvent.AddListener(OnCameraActivated);
    }

    private void OnDisable()
    {
        CinemachineCore.Instance.GetActiveBrain(0).m_CameraActivatedEvent
            .RemoveListener(OnCameraActivated);
    }

    private void OnCameraActivated(ICinemachineCamera oldCamera, ICinemachineCamera newCamera)
    {
        if(IsFPS) onSwitchedToFPS.Invoke();
        else onSwitchedToTPS.Invoke();
    }

    public override void OnNetworkSpawn()
    {
        base.OnNetworkSpawn();
        Debug.Log($"Am I the owner of {OwnerClientId}? {IsOwner}");
        name = $"Player {OwnerClientId}";
        Debug.Log("Am I the owner? " + IsOwner);
        if (IsOwner)
        {
            AssignCamera("FPSVirtualCamera", fpsFollowTarget);
            AssignCamera("TPSVirtualCamera", tpsFollowTarget);
        }
        else
        {
            foreach (var component in localPlayerBehaviours)
            {
                component.enabled = false;
            }

            foreach (var component in remotePlayerBehaviours)
            {
                component.enabled = true;
            }
        }
    }

    private void AssignCamera(string virtualCameraName, Transform target)
    {
        var playerCameras = GameObject.FindGameObjectsWithTag(virtualCameraName);
        foreach (var playerCamera in playerCameras)
        {
            var virtualCamera = playerCamera.GetComponentInChildren<CinemachineVirtualCamera>();
            virtualCamera.Follow = target;
        }
    }
}
