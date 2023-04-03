using UnityEngine;
using UnityEngine.AI;

namespace DoubTech.Multiplayer.NPCs
{
    public class VelocityTracker : MonoBehaviour, IVelocityTracker
    {
        public Vector3 LinearVelocity { get; private set; }
        public Vector3 AngularVelocity { get; private set; }
        public float Speed { get; private set; }
        public float TurnSpeed { get; private set; }

        private Vector3 previousPosition;
        private Quaternion previousRotation;
        private NavMeshAgent navMeshAgent;

        void Start()
        {
            previousPosition = transform.position;
            previousRotation = transform.rotation;
        }

        void Update()
        {
            if (navMeshAgent)
            {
                LinearVelocity = navMeshAgent.velocity;
                Speed = LinearVelocity.magnitude;
            }
            else
            {
                // Calculate linear velocity
                LinearVelocity = (transform.position - previousPosition) / Time.fixedDeltaTime;
                previousPosition = transform.position;
                Speed = LinearVelocity.magnitude;
            }

            // Calculate angular velocity
            Quaternion deltaRotation = transform.rotation * Quaternion.Inverse(previousRotation);
            deltaRotation.ToAngleAxis(out float angleInDegrees, out Vector3 rotationAxis);
            float angleInRadians = Mathf.Deg2Rad * angleInDegrees;
            AngularVelocity = rotationAxis * angleInRadians / Time.fixedDeltaTime;
            previousRotation = transform.rotation;
            TurnSpeed = AngularVelocity.magnitude;
        }
    }
    
    public interface IVelocityTracker
    {
        Vector3 LinearVelocity { get; }
        Vector3 AngularVelocity { get; }
        float Speed { get; }
        float TurnSpeed { get; }
    }
}