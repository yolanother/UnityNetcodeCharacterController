using System;
using UnityEngine;

namespace StarterAssets
{
    public class AnimationEventHandler : MonoBehaviour
    {
        private ThirdPersonController _controller;

        private void Start()
        {
            _controller = GetComponentInParent<ThirdPersonController>();
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            _controller?.OnFootstep(animationEvent);
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            _controller?.OnLand(animationEvent);
        }
    }
}