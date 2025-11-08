using System;
using Clickbait.Utilities;
using UnityEngine;
using Random = UnityEngine.Random;

/* Note: animations are called via the controller for both the character and capsule using animator null checks
 */

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Input")]
        [SerializeField] InputReader _inputReader;
        [field: SerializeField] public bool IsPlayer { get; private set; } = true;
        public ICharacterInput CharacterInput { get; private set; }
        
        [Header("Agent")]
        [Tooltip("Use the input reader?")]
        [SerializeField] bool _debugInput;
        [SerializeField] Transform _animationAimTargetParent;
        [SerializeField] bool _isDummy;
        
        [Header("Player")]
        [Tooltip("Move speed of the character in m/s")]
        public float MoveSpeed = 2.0f;

        [Tooltip("Sprint speed of the character in m/s")]
        public float SprintSpeed = 5.335f;
        public float AimWalkSpeed = 2.5f;

        [Tooltip("How fast the character turns to face movement direction")]
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;

        [Tooltip("Acceleration and deceleration")]
        public float SpeedChangeRate = 10.0f;

        public AudioClip LandingAudioClip;
        public AudioClip[] FootstepAudioClips;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        [Tooltip("The height the player can jump")]
        public float JumpHeight = 1.2f;

        [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
        public float Gravity = -15.0f;

        [Space(10)]
        [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
        public float JumpTimeout = 0.50f;

        [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
        public float FallTimeout = 0.15f;

        [Header("Player Grounded")]
        [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
        public bool Grounded = true;

        [Tooltip("Useful for rough ground")]
        public float GroundedOffset = -0.14f;

        [Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
        public float GroundedRadius = 0.28f;

        [Tooltip("What layers the character uses as ground")]
        public LayerMask GroundLayers;

        [Header("Cinemachine")]
        [Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
        public GameObject CinemachineCameraTarget;

        [Tooltip("How far in degrees can you move the camera up")]
        public float TopClamp = 70.0f;

        [Tooltip("How far in degrees can you move the camera down")]
        public float BottomClamp = -30.0f;

        [Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
        public float CameraAngleOverride = 0.0f;

        [Tooltip("For locking the camera position on all axis")]
        public bool LockCameraPosition = false;
        float _sensitivity = 1f; // Don't change - it's set in ThirdPersonShooterController
        [SerializeField] RigManager _rigManager;
        
        // cinemachine
        private float _cinemachineTargetYaw;
        private float _cinemachineTargetPitch;

        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        int _animIDSprinting;
        int _animIDStrafing;
        int _animIDTuckGun;
        int _animIDIsStrafing;

        [SerializeField] Animator _animator;
        private CharacterController _controller;
        [SerializeField] GameObject _mainCamera;
        CharacterAnimationLayerManager _animationLayerManager;

        private const float LookThreshold = 0.01f;

        private bool _hasAnimator;
        [SerializeField] bool _rotateOnMove = true;

        public Observer<bool> IsSprinting = new(false);

        public Observer<bool> CanShoot = new(true);
        bool _dead;
        Health _health;

        bool _isReloading;
        bool _isHeuristic;

        public Vector3 Velocity => _controller.velocity;
        
        private void Awake()
        {
            // get a reference to our main camera
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
            }

            if (IsPlayer || _debugInput)
            {
                CharacterInput = _inputReader;
            } else
            {
                CharacterInput = GetComponent<AgentInput>();
            }

            _health = GetComponent<Health>();
            _hasAnimator = _animator != null;
            _controller = GetComponent<CharacterController>();
            _animationLayerManager = GetComponent<CharacterAnimationLayerManager>();
        }

        private void Start()
        {
            InitializeVariables();
            AssignAnimationIDs();
            
            InitialSubscriptions();
            
            CharacterInput.Enable();
        }

        public void OnHeuristicEnable() => _isHeuristic = true;
        
        void InitialSubscriptions()
        {
            if (IsPlayer || _isHeuristic)
            {
                CursorStateSubscription();
            }
            
            IsSprinting.AddListener(sprint => _animator.SetBool(_animIDSprinting, sprint));
            if (IDeath.IsFreezeOnDeath())
            {
                _health.Death.OnDeath += OnDie;
            }
        }

        void CursorStateSubscription()
        {
            CharacterInput.IsPaused.AddListener(isPaused =>
            {
                if (isPaused)
                {
                    UnlockCursor();
                } else
                {
                    LockCursor();
                }
            });
        }

        static void LockCursor()
        {
            if (GameManager.Instance.IsGameOver) return;
            
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        }

        public static void UnlockCursor()
        {
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }

        void OnDie()
        {
            _dead = true;
        }

        void InitializeVariables()
        {
            _cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
            
            // reset our timeouts on start
            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        public void TuckGun()
        {
            _rigManager.SetWeightSmoothed(0);
            _animator.SetBool(_animIDTuckGun, true);
            CanShoot.Value = false;
        }

        public void UntuckGun()
        {
            _rigManager.SetWeightSmoothed(1);
            _animator.SetBool(_animIDTuckGun, false);
            CanShoot.Value = true;
        }

        void Update()
        {
            JumpAndGravity();
            GroundedCheck();
            Move();
        }

        private void LateUpdate()
        {
            CameraRotation();
        }

        private void AssignAnimationIDs()
        {
            _animIDSpeed = Animator.StringToHash("Speed");
            _animIDGrounded = Animator.StringToHash("Grounded");
            _animIDJump = Animator.StringToHash("Jump");
            _animIDFreeFall = Animator.StringToHash("FreeFall");
            _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
            _animIDSprinting = Animator.StringToHash("Sprinting");
            _animIDStrafing = Animator.StringToHash("Strafe");
            _animIDTuckGun = Animator.StringToHash("TuckGun");
            _animIDIsStrafing = Animator.StringToHash("IsStrafing");
        }

        private void GroundedCheck()
        {
            // set sphere position, with offset
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
        }

        private void CameraRotation()
        {
            // if there is an input and camera position is not fixed
            if (CharacterInput.Look.sqrMagnitude >= LookThreshold && !LockCameraPosition)
            {
                // Don't multiply mouse input by Time.deltaTime;
                float deltaTimeMultiplier = 1;
                if (_inputReader.AnalogMovement && IsPlayer)
                    deltaTimeMultiplier = Time.deltaTime;

                _cinemachineTargetYaw += CharacterInput.Look.x * deltaTimeMultiplier * _sensitivity;
                _cinemachineTargetPitch += CharacterInput.Look.y * deltaTimeMultiplier * _sensitivity;
            }

            // clamp our rotations so our values are limited 360 degrees
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
            
            if (!GameManager.Instance.IsGameOver)
            {
                if (IsPlayer)
                {
                    // Cinemachine will follow this target
                    CinemachineCameraTarget.transform.rotation = Quaternion.Euler(
                        _cinemachineTargetPitch + CameraAngleOverride,
                        _cinemachineTargetYaw, 0.0f);
                } else
                {
                    Vector3 pitchRotation = new Vector3(_cinemachineTargetPitch + CameraAngleOverride, 0, 0);

                    transform.eulerAngles = transform.eulerAngles.With(y: _cinemachineTargetYaw);
                    _animationAimTargetParent.localEulerAngles = pitchRotation;

                    // Not needed - just so that you can use the CinemachineTarget gameObject to watch the agent
                    CinemachineCameraTarget.transform.localEulerAngles = pitchRotation;
                }
            }
        }
        
        private void Move()
        {
            if (_dead) return;
            
            // set target speed based on move speed, sprint speed and if sprint is pressed
            float targetSpeed;

            // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

            // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is no input, set the target speed to 0
            if (CharacterInput.Move == Vector2.zero)
            {
                // No movement
                targetSpeed = 0.0f;
                IsSprinting.Value = false;
            } else
            {
                // Moving
                if (CharacterInput.Sprint)
                {
                    // We're pressing the sprint button
                    if (CharacterInput.IsAiming.Value)
                    {
                        targetSpeed = AimWalkSpeed;
                        IsSprinting.Value = false;
                    }
                    else if (_inputReader.IsShooting.Value || _isReloading)
                    {
                        // Can't sprint because you're shooting/reloading
                        targetSpeed = MoveSpeed;
                        IsSprinting.Value = false;
                    } else
                    {
                        targetSpeed = SprintSpeed;
                        IsSprinting.Value = true;
                    }
                } else
                {
                    // We're not pressing the sprint button
                    targetSpeed = CharacterInput.IsAiming.Value ? AimWalkSpeed : MoveSpeed;
                    IsSprinting.Value = false;
                }
            }
            
            HandleStrafeAnimation();

            // a reference to the players current horizontal velocity
            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

            float speedOffset = 0.1f;
            float inputMagnitude = CharacterInput.AnalogMovement ? CharacterInput.Move.magnitude : 1f;

            // accelerate or decelerate to target speed
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                // creates curved result rather than a linear one giving a more organic speed change
                // note T in Lerp is clamped, so we don't need to clamp our speed
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);

                // round speed to 3 decimal places
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
            }
            else
            {
                _speed = targetSpeed;
            }

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;

            // normalise input direction
            Vector3 inputDirection = new Vector3(CharacterInput.Move.x, 0.0f, CharacterInput.Move.y).normalized;
            
            // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
            // if there is a move input rotate player when the player is moving
            if (CharacterInput.Move != Vector2.zero)
            {
                float yAngle = _isDummy ? 0 : _mainCamera.transform.eulerAngles.y; // If dummy, use global directions, not camera-based
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + yAngle;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                // rotate to face input direction relative to camera position
                if (_rotateOnMove)
                {
                    transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                }
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            // move the player
            _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                             new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
            }
        }

        void HandleStrafeAnimation()
        {
            float strafeAmount = CharacterInput.Move.x * (IsSprinting.Value ? 2.0f : 1.0f);
            _animator.SetFloat(_animIDStrafing, strafeAmount);
            
            bool isStrafing = CharacterInput.Move.x != 0 && CharacterInput.Move.y == 0; // True if we're moving only sideways
            _animator.SetBool(_animIDIsStrafing, isStrafing);
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                // reset the fall timeout timer
                _fallTimeoutDelta = FallTimeout;

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                // stop our velocity dropping infinitely when grounded
                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                // Jump
                if (CharacterInput.Jump && _jumpTimeoutDelta <= 0.0f)
                {
                    // the square root of H * -2 * G = how much velocity needed to reach desired height
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }

                // jump timeout
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                // reset the jump timeout timer
                _jumpTimeoutDelta = JumpTimeout;

                // fall timeout
                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    // update animator if using character
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                // if we are not grounded, do not jump
                //_characterInput.Jump = false;
            }

            // apply gravity over time if under terminal (multiply by delta time twice to linearly speed up over time)
            if (_verticalVelocity < _terminalVelocity)
            {
                _verticalVelocity += Gravity * Time.deltaTime;
            }
        }

        private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
        {
            if (lfAngle < -360f) lfAngle += 360f;
            if (lfAngle > 360f) lfAngle -= 360f;
            return Mathf.Clamp(lfAngle, lfMin, lfMax);
        }

        private void OnDrawGizmosSelected()
        {
            Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
            Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

            if (Grounded) Gizmos.color = transparentGreen;
            else Gizmos.color = transparentRed;

            // when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        public void OnFootstep(AnimationEvent animationEvent)
        {
            if (TrainingManager.Instance.IsTraining) return;
            
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioSource.PlayClipAtPoint(FootstepAudioClips[index], transform.TransformPoint(_controller.center), FootstepAudioVolume);
                }
            }
        }

        public void OnLand(AnimationEvent animationEvent)
        {
            if (TrainingManager.Instance.IsTraining) return;
            
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioSource.PlayClipAtPoint(LandingAudioClip, transform.TransformPoint(_controller.center), FootstepAudioVolume);
            }
        }

        public void SetSensitivity(float sensitivity) => _sensitivity = sensitivity;
        public void SetIsReloading(bool isReloading) => _isReloading =  isReloading;
    }
}