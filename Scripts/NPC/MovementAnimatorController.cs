using UnityEngine;

namespace DoubTech.Multiplayer.NPCs
{
    public class MovementAnimatorController : MonoBehaviour
    {
        public enum MovementMode
        {
            Strafe,
            Directional
        }

        [Header("Mode")]
        [SerializeField] private MovementMode movementMode = MovementMode.Strafe;

        [Header("Parameters")]
        [SerializeField] private Animator animator;
        [SerializeField] private VelocityTracker velocityTracker;

        [Header("Parameter Names")]
        [SerializeField] private string speedParameter = "Speed";
        [SerializeField] private string turnParameter = "Turn";
        [SerializeField] private string horizontalParameter = "Horizontal";
        [SerializeField] private string verticalParameter = "Vertical";
        [SerializeField] private string motionSpeedParameter = "MotionSpeed";

        [Header("Lerping")]
        [SerializeField] private float lerpSpeed = 10.0f;

        public Animator Animator
        {
            get => animator;
            set => animator = value;
        }
        
        private int speedParameterHash;
        private int turnParameterHash;
        private int horizontalParameterHash;
        private int verticalParameterHash;
        private int motionSpeedParameterHash;

        private float currentSpeed;
        private float currentTurnSpeed;
        private float currentHorizontal;
        private float currentVertical;
        private float currentMotionSpeed;

        private void Start()
        {
            speedParameterHash = Animator.StringToHash(speedParameter);
            turnParameterHash = Animator.StringToHash(turnParameter);
            horizontalParameterHash = Animator.StringToHash(horizontalParameter);
            verticalParameterHash = Animator.StringToHash(verticalParameter);
            motionSpeedParameterHash = Animator.StringToHash(motionSpeedParameter);
        }

        private void Update()
        {
            if (!animator.enabled) return;
            if (!animator.gameObject.activeInHierarchy) return;
            
            if (movementMode == MovementMode.Strafe)
            {
                float targetHorizontal = velocityTracker.LinearVelocity.x;
                currentHorizontal = Mathf.Lerp(currentHorizontal, targetHorizontal, Time.deltaTime * lerpSpeed);
                animator.SetFloat(horizontalParameterHash, currentHorizontal);

                float targetVertical = velocityTracker.LinearVelocity.z;
                currentVertical = Mathf.Lerp(currentVertical, targetVertical, Time.deltaTime * lerpSpeed);
                animator.SetFloat(verticalParameterHash, currentVertical);
            }
            else if (movementMode == MovementMode.Directional)
            {
                float targetSpeed = velocityTracker.Speed;
                currentSpeed = Mathf.Lerp(currentSpeed, targetSpeed, Time.deltaTime * lerpSpeed);
                animator.SetFloat(speedParameterHash, currentSpeed);
            }
            
            animator.SetFloat(turnParameterHash, velocityTracker.TurnSpeed);

            animator.SetFloat(motionSpeedParameterHash, velocityTracker.Speed < 0 ? -1 : 1);
        }
    }
}