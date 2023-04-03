using System;
using UnityEngine;
using UnityEngine.Events;

namespace Utilities
{
    public class OnEnableEvent : MonoBehaviour
    {
        [SerializeField] private UnityEvent onEnable = new UnityEvent();
        [SerializeField] private UnityEvent onDisable = new UnityEvent();
        
        private void OnEnable()
        {
            onEnable?.Invoke();
        }

        private void OnDisable()
        {
            onDisable?.Invoke();
        }
    }
}