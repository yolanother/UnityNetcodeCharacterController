using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

namespace DoubTech.Multiplayer.NPCs
{
    public class NPCMover : MonoBehaviour
    {
        public enum MovementSpeed
        {
            Walk,
            Run,
            Sprint
        }
        
        public enum MovementState
        {
            Idle,
            Moving
        }

        public enum TargetingMode
        {
            Transform,
            Position
        }

        [Header("Target")]
        [SerializeField] private Transform _target;
        [SerializeField] private TargetingMode targetingMode = TargetingMode.Transform;
        [SerializeField] private Vector3 targetPosition;

        [Header("Movement Speed")]
        [SerializeField] private MovementSpeed movementSpeed = MovementSpeed.Walk;
        [SerializeField] private float walkSpeed = 1f;
        [SerializeField] private float runSpeed = 3f;
        [SerializeField] private float sprintSpeed = 5f;

        [Header("Stopping Distance and Rotation Speed")]
        [SerializeField] private float stoppingDistance = 0.5f;
        [SerializeField] private float rotationSpeed = 5f;
        
        [Header("Stuck Detection")]
        [SerializeField] private float stuckThreshold = 0.05f;
        [SerializeField] private float checkStuckInterval = 1f;
        public UnityEvent onStuck = new UnityEvent();
        public UnityEvent onReachedDestination = new UnityEvent();

        private NavMeshAgent navMeshAgent;
        private bool hasNavMeshAgent;

        private Vector3 previousPosition;
        private float stuckCheckTimer;
        
        private MovementState currentMovementState = MovementState.Idle;

        void Start()
        {
            navMeshAgent = GetComponent<NavMeshAgent>();
            hasNavMeshAgent = navMeshAgent != null;
            previousPosition = transform.position;
            stuckCheckTimer = 0f;
            if (targetingMode == TargetingMode.Position && DistanceToTarget > stoppingDistance)
            {
                CurrentMovementState = MovementState.Moving;
            }
        }

        void Update()
        {
            Vector3 destination = targetingMode == TargetingMode.Transform ? _target.position : targetPosition;
            float distanceToTarget = Vector3.Distance(transform.position, destination);

            if (distanceToTarget <= stoppingDistance && CurrentMovementState != MovementState.Idle)
            {
                CurrentMovementState = MovementState.Idle;
                onReachedDestination.Invoke();
            }
            else if (distanceToTarget > stoppingDistance && CurrentMovementState == MovementState.Idle)
            {
                CurrentMovementState = MovementState.Moving;
            }

            if (hasNavMeshAgent)
            {
                if (navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && distanceToTarget > stoppingDistance)
                {
                    navMeshAgent.ResetPath();
                    navMeshAgent.SetDestination(destination);
                }
                
                // Move using NavMeshAgent
                navMeshAgent.destination = destination;

                // Set the agent's speed according to its current state (walk, run, or sprint)
                navMeshAgent.speed = Speed;

                // Set the stopping distance
                navMeshAgent.stoppingDistance = stoppingDistance;

                // Rotate the GameObject along the path of the NavMesh
                if (navMeshAgent.hasPath && navMeshAgent.pathStatus == NavMeshPathStatus.PathComplete && navMeshAgent.desiredVelocity.magnitude > 0)
                {
                    Quaternion targetRotation = Quaternion.LookRotation(navMeshAgent.desiredVelocity);
                    transform.rotation =
                        Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
                }
            }
            else if (distanceToTarget > stoppingDistance)
            {
                // Move directly towards the target
                Vector3 direction = (destination - transform.position).normalized;
                transform.position += direction * Speed * Time.deltaTime;

                // Rotate the GameObject towards the target
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                transform.rotation =
                    Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
            }

            CheckIfStuck();
        }
        
        private void CheckIfStuck()
        {
            if (Speed <= 0f) return;

            stuckCheckTimer += Time.deltaTime;

            if (stuckCheckTimer >= checkStuckInterval)
            {
                float distanceMoved = Vector3.Distance(previousPosition, transform.position);

                if (distanceMoved < stuckThreshold)
                {
                    onStuck.Invoke();
                    if (hasNavMeshAgent)
                    {
                        navMeshAgent.isStopped = true;
                    }
                }
                else
                {
                    if (hasNavMeshAgent)
                    {
                        navMeshAgent.isStopped = false;
                    }
                }

                previousPosition = transform.position;
                stuckCheckTimer = 0f;
            }
        }

        public float DistanceToTarget
        {
            get
            {
                Vector3 destination = targetingMode == TargetingMode.Transform ? _target.position : targetPosition;
                return Vector3.Distance(transform.position, destination);
            }
        }

        public MovementSpeed DesiredMovementSpeed
        {
            get => movementSpeed;
            set => movementSpeed = value;
        }
        
        public MovementState CurrentMovementState
        {
            get => currentMovementState;
            set => currentMovementState = value;
        }

        private float Speed
        {
            get
            {
                switch (DesiredMovementSpeed)
                {
                    case MovementSpeed.Walk:
                        return walkSpeed;
                    case MovementSpeed.Run:
                        return runSpeed;
                    case MovementSpeed.Sprint:
                        return sprintSpeed;
                    default:
                        return 0;
                }
            }
        }

        public Transform Target
        {
            get => _target;
            set
            {
                _target = value;
                targetingMode = TargetingMode.Transform;
                CurrentMovementState = MovementState.Moving;
            }
        }

        public Vector3 TargetPosition
        {
            get => targetPosition;
            set
            {
                targetPosition = value;
                targetingMode = TargetingMode.Position;
                CurrentMovementState = MovementState.Moving;
            }
        }
    }
}
