using System.Collections.Generic;
using DoubTech.Multiplayer.NPCs;
using UnityEngine;

namespace Footsteps
{
    [RequireComponent(typeof(AudioSource))]
    public class FootstepHandler : MonoBehaviour
    {
        [SerializeField] private List<AudioClip> footstepSounds;
        [SerializeField] private VelocityTracker velocityTracker;
        [SerializeField] private float maxSpeed = 1;

        private AudioSource audioSource;

        private void Awake()
        {
            audioSource = GetComponent<AudioSource>();
        }

        public void OnFootstep()
        {
            if (footstepSounds != null && footstepSounds.Count > 0)
            {
                int randomIndex = Random.Range(0, footstepSounds.Count);
                AudioClip clip = footstepSounds[randomIndex];
                audioSource.volume = Mathf.Clamp(velocityTracker.Speed, 0, maxSpeed) / maxSpeed;
                audioSource.PlayOneShot(clip);
            }
        }
    }
}