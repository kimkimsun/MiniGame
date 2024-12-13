using UnityEngine;
#if ENABLE_INPUT_SYSTEM 
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
    [RequireComponent(typeof(CharacterController))]
#if ENABLE_INPUT_SYSTEM 
    [RequireComponent(typeof(PlayerInput))]
#endif
    public class ThirdPersonController : MonoBehaviour
    {
        [Header("Player")]
        public float MoveSpeed = 2.0f;
        public float SprintSpeed = 5.335f;
        [Range(0.0f, 0.3f)]
        public float RotationSmoothTime = 0.12f;
        public float SpeedChangeRate = 10.0f;
        public GameObject LandingAudioClip;
        public GameObject[] FootstepAudioClips;
        public Transform    playerSpine;
        [Range(0, 1)] public float FootstepAudioVolume = 0.5f;

        [Space(10)]
        public float JumpHeight = 1.2f;
        public float Gravity = -15.0f;
        [Space(10)]
        public float JumpTimeout = 0.50f;
        public float FallTimeout = 0.05f;

        [Header("Player Grounded")]
        public bool Grounded = true;
        public float GroundedOffset = -0.14f;
        public float GroundedRadius = 0.28f;
        public LayerMask GroundLayers;
        [Header("Cinemachine")]
        public GameObject CinemachineCameraThirdTarget;
        public float TopClamp = 70.0f;
        public float BottomClamp = -30.0f;
        public float firstTopClamp = 0;
        public float firstBottomClamp = 0;
        public float CameraAngleOverride = 0.0f;
        public bool LockCameraPosition = false;
        public bool isAimMove;
        public bool isReload;

        // cinemachine
        private float _cinemachineTargetYaw;
        public float _cinemachineTargetPitch;
        private float _cinemachineFirstTargetYaw;
        private float _cinemachineFirstTargetPitch;



        // player
        private float _speed;
        private float _animationBlend;
        private float _targetRotation = 0.0f;
        private float _rotationVelocity;
        private float _verticalVelocity;
        private float _terminalVelocity = 53.0f;
        private float aimRotationYFix = 24.5f;

        // timeout deltatime
        private float _jumpTimeoutDelta;
        private float _fallTimeoutDelta;

        // animation IDs
        private int _animIDSpeed;
        private int _animIDGrounded;
        private int _animIDJump;
        private int _animIDCrouch;
        private int _animIDFreeFall;
        private int _animIDMotionSpeed;
        private int _animIDAim;

#if ENABLE_INPUT_SYSTEM 
        private PlayerInput _playerInput;
#endif
        private Animator _animator;
        private CharacterController _controller;
        private StarterAssetsInputs _input;
        private GameObject _mainCamera;

        private const float _threshold = 0.01f;

        private bool _hasAnimator;
        private bool IsCurrentDeviceMouse
        {
            get
            {
#if ENABLE_INPUT_SYSTEM
                return _playerInput.currentControlScheme == "KeyboardMouse";
#else
				return false;
#endif
            }
        }


        private void Awake()
        {
            if (_mainCamera == null)
            {
                _mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
                // 카메라 설정 근데 끌어오는 게 낫지 않나? 아무튼
            }
        }

        private void Start()
        {
            _cinemachineTargetYaw = CinemachineCameraThirdTarget.transform.rotation.eulerAngles.y;

            _hasAnimator = TryGetComponent(out _animator);
            _controller = GetComponent<CharacterController>();
            _input = GetComponent<StarterAssetsInputs>();
#if ENABLE_INPUT_SYSTEM 
            _playerInput = GetComponent<PlayerInput>();
#else
#endif

            AssignAnimationIDs();

            _jumpTimeoutDelta = JumpTimeout;
            _fallTimeoutDelta = FallTimeout;
        }

        private void Update()
        {
            _hasAnimator = TryGetComponent(out _animator);

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
            _animIDAim = Animator.StringToHash("Aim");
            _animIDCrouch = Animator.StringToHash("Crouch");
        }

        private void GroundedCheck()
        {
            Vector3 spherePosition = new Vector3(transform.position.x, transform.position.y - GroundedOffset,
                transform.position.z);
            Grounded = Physics.CheckSphere(spherePosition, GroundedRadius, GroundLayers,
                QueryTriggerInteraction.Ignore);

            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, Grounded);
            }
            // 트리거 충돌은 무시하고 바닥을 체크하는 것으로
            // 바닥에 충돌을 본 캐릭터보다 먼저하므로 그에 따른 애니메이션이 먼저 실행된다.
        }

        private void CameraRotation()
        {
            if (_input.look.sqrMagnitude >= _threshold && !LockCameraPosition)
                // 내가 바라보는 방향의 제곱을 나타낸다 그것이 threshold 즉 0.01f보다 높고, LockCameraPosition이 false일 때만 안에 들어온다.
                // threshold는 데드존이다. 입력 값이 무시 될 정도로 적은 값을 뜻함
            {
                float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

                // x와 y값을 반환 해줍니다.
                // 마우스라면 1.0을 반환하며 X와 Y에 대한 속도를 정해준다.
                _cinemachineTargetYaw += _input.look.x * deltaTimeMultiplier;
                _cinemachineTargetPitch += _input.look.y * deltaTimeMultiplier;
                //_cinemachineFirstTargetYaw += _input.look.x * deltaTimeMultiplier;
                //_cinemachineFirstTargetPitch += _input.look.y * deltaTimeMultiplier;
            }

            // 좌우 상화에 대한 각도를 제한 하는 함수
            _cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
            _cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);
           // _cinemachineFirstTargetYaw = ClampAngle(_cinemachineFirstTargetYaw, float.MinValue, float.MaxValue);
           // _cinemachineFirstTargetPitch = ClampAngle(_cinemachineFirstTargetPitch, firstBottomClamp, firstTopClamp);
            // float.minValue와 max는 float으로 표현할 수 있는 가장 작은 수와 가장 큰 수를 뜻한다. 즉, 한계가 없다는 것
            // -999999999, 99999999999이런거와 똑같음

           

            CinemachineCameraThirdTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
                _cinemachineTargetYaw, 0.0f);
            Quaternion currentRotation = playerSpine.rotation;
            float currentY = currentRotation.eulerAngles.y;
            float currentZ = currentRotation.eulerAngles.z;
            if(_input.aim) playerSpine.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride, currentY+ aimRotationYFix, currentZ);
            // 그에 따른 target의 회전각을 바꾼다.
        }

        private void Move()
        {
            float targetSpeed = _input.sprint ? SprintSpeed : MoveSpeed;
            if (_input.crouch)
            {
                _animator.SetBool(_animIDCrouch, true);
                targetSpeed = 2;
            }
            else
            {
                _animator.SetBool(_animIDCrouch, false);
            }
            // 캐릭터의 이동 속도가 Shift를 눌렀을 때와 안 눌렀을 때의 구분
            if (isAimMove || isReload) targetSpeed = MoveSpeed;
            // 조준 중이거나 Reload중이라면 Shift를 누르더라도 걷는 속도
            if (_input.move == Vector2.zero) targetSpeed = 0.0f;
            // 안움직이다면 0으로 초기화

            float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;
            // magnitude를 하여 벡터의 크기를 반환받고 currentHorizontalSpeed에 넣음

            float speedOffset = 0.1f;
            // 목표 속도와 비교할 때의 오차 허용 범위
            float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;
            // 조이스틱을 사용한다면 그것의 세기를 받아들인다.
            if (currentHorizontalSpeed < targetSpeed - speedOffset ||
                currentHorizontalSpeed > targetSpeed + speedOffset)
            {
                _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
                    Time.deltaTime * SpeedChangeRate);
                _speed = Mathf.Round(_speed * 1000f) / 1000f;
                // 속도를 보간 하는 것 차이가 너무 벌어지는 것을 방지
            }
            else   _speed = targetSpeed; // 속도의 차이가 너무 나지 않는다면 그냥 그대로 쓰십쇼

            _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
            if (_animationBlend < 0.01f) _animationBlend = 0f;
            // 애니메이션의 블렌드 속도를 보간하여 부드럽게 전환하도록 함
            // 이래서 타겟스피드랑 threshold랑 맞춘거구나
            // 매우 작아진다면 그냥 0으로 바꿈
            
            Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;
            // input값으로 입력받는 것들에 대한 방향만을 가져옵니다


            if (_input.move != Vector2.zero)
                // 만약 움직이고 있다면
            {
                _targetRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg +
                                  _mainCamera.transform.eulerAngles.y;
                float rotation = Mathf.SmoothDampAngle(transform.eulerAngles.y, _targetRotation, ref _rotationVelocity,
                    RotationSmoothTime);

                if(!isAimMove) transform.rotation = Quaternion.Euler(0.0f, rotation, 0.0f);
                // 바라보고 있는 방향을 부드럽게 전환 하는데 조준을 안하고 있을때만 가능하게끔
            }


            Vector3 targetDirection = Quaternion.Euler(0.0f, _targetRotation, 0.0f) * Vector3.forward;

            if (_controller.enabled)
            {
                _controller.Move(targetDirection.normalized * (_speed * Time.deltaTime) +
                                 new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
                // 움직입니다.
            }
            if (_hasAnimator)
            {
                _animator.SetFloat(_animIDSpeed, _animationBlend);
                _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
                // 애니메이터 파라미터의 value값
            }
        }

        private void JumpAndGravity()
        {
            if (Grounded)
            {
                _fallTimeoutDelta = FallTimeout;

                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, false);
                    _animator.SetBool(_animIDFreeFall, false);
                }

                if (_verticalVelocity < 0.0f)
                {
                    _verticalVelocity = -2f;
                }

                if (_input.jump && _jumpTimeoutDelta <= 0.0f)
                {
                    _verticalVelocity = Mathf.Sqrt(JumpHeight * -2f * Gravity);

                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDJump, true);
                    }
                }
                if (_jumpTimeoutDelta >= 0.0f)
                {
                    _jumpTimeoutDelta -= Time.deltaTime;
                }
            }
            else
            {
                _jumpTimeoutDelta = JumpTimeout;

                if (_fallTimeoutDelta >= 0.0f)
                {
                    _fallTimeoutDelta -= Time.deltaTime;
                }
                else
                {
                    if (_hasAnimator)
                    {
                        _animator.SetBool(_animIDFreeFall, true);
                    }
                }

                _input.jump = false;
            }

            if (_verticalVelocity < _terminalVelocity)   _verticalVelocity += Gravity * Time.deltaTime;
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

            Gizmos.DrawSphere(
                new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
                GroundedRadius);
        }

        private void OnFootstep(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                if (FootstepAudioClips.Length > 0)
                {
                    var index = Random.Range(0, FootstepAudioClips.Length);
                    AudioManager.Instance.PlaySound(FootstepAudioClips[index], this.transform.position);
                }
            }
        }

        private void OnLand(AnimationEvent animationEvent)
        {
            if (animationEvent.animatorClipInfo.weight > 0.5f)
            {
                AudioManager.Instance.PlaySound(LandingAudioClip, this.transform.position);
            }
        }
    }
}