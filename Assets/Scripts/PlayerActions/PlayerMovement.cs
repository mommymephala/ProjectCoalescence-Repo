using UnityEngine;

namespace PlayerActions
{
    public class PlayerMovement : MonoBehaviour
    {
        private const float PlayerHeight = 2f;
        [SerializeField] private Transform orientation;
        [SerializeField] private Camera playerCam;

        [Header("Movement")]
        private const float MovementMultiplier = 10f;
        [SerializeField] private float moveSpeed = 1f;
        [SerializeField] private float airMultiplier = 1f;
        [SerializeField] private Vector3 gravity;

        [Header("Sprinting")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] private float sprintSpeed = 6f;
        [SerializeField] private float acceleration = 10f;
        public bool IsSprinting { get; private set; }
        public bool IsWalking { get; private set; }

        [Header("Headbob")]
        [SerializeField] private bool toggleHeadbob = true;
        [SerializeField] private float headbobFrequency = 2f;
        [SerializeField] private float walkHeadbobAmount = 0.1f;
        [SerializeField] private float sprintHeadbobAmount = 0.3f;
        [SerializeField] private float headbobSpeedMultiplier = 1f;
        private float _headbobTimer;
        private Vector3 _originalLocalPosition;

        [Header("Keybinds")]
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Drag")]
        [SerializeField] private float groundDrag = 6f;
        [SerializeField] private float airDrag = 2f;
        private float _horizontalMovement;
        private float _verticalMovement;

        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance = 0.2f;
        private bool _isGrounded;
        private Vector3 _moveDirection;
        private Vector3 _slopeMoveDirection;
        private Rigidbody _rb;
        private RaycastHit _slopeHit;
        private bool _isCameraNotNull;

        private void Awake()
        {
            _isCameraNotNull = playerCam != null;
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
            if (playerCam != null)
            {
                _originalLocalPosition = playerCam.transform.localPosition;
            }
        }

        private void Update()
        {
            _isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

            TakeInput();
            ControlDrag();
            ControlSpeed();
            HandleHeadbob();

            _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);
        }

        private void FixedUpdate()
        {
            MovePlayer();
            ApplyGravity();
        }

        private void TakeInput()
        {
            _horizontalMovement = Input.GetAxisRaw("Horizontal");
            _verticalMovement = Input.GetAxisRaw("Vertical");

            _moveDirection = orientation.forward * _verticalMovement + orientation.right * _horizontalMovement;
            IsWalking = !Input.GetKey(sprintKey) && (_horizontalMovement != 0f || _verticalMovement != 0f);
        }

        private void ControlSpeed()
        {
            if (Input.GetKey(sprintKey) && _isGrounded)
            {
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
                IsSprinting = true;
            }
            else
            {
                moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
                IsSprinting = false;
            }
        }

        private void ControlDrag()
        {
            _rb.drag = _isGrounded ? groundDrag : airDrag;
        }

        private bool OnSlope()
        {
            if (!Physics.Raycast(transform.position, Vector3.down, out _slopeHit, PlayerHeight / 2 + 0.5f))
                return false;
            return _slopeHit.normal != Vector3.up;
        }

        private void MovePlayer()
        {
            switch (_isGrounded)
            {
                case true when !OnSlope():
                    _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
                    break;
                case true when OnSlope():
                    _rb.AddForce(_slopeMoveDirection.normalized * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
                    break;
                case false:
                    _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier * airMultiplier), ForceMode.Acceleration);
                    break;
            }
        }

        private void ApplyGravity()
        {
            if (!_isGrounded)
            {
                _rb.AddForce(gravity, ForceMode.Acceleration);
            }
        }
        
        //do it better
        private void HandleHeadbob()
        {
            if (toggleHeadbob && _isGrounded && moveSpeed > 0 && (_horizontalMovement != 0 || _verticalMovement != 0))
            {
                // Calculate headbob movement using a sinusoidal curve
                var bobAmount = IsSprinting ? sprintHeadbobAmount : walkHeadbobAmount;
                var bobAmountX = Mathf.Sin(_headbobTimer) * bobAmount;
                var bobAmountY = Mathf.Cos(_headbobTimer * 2) * bobAmount * 0.5f;

                // Apply headbob only in the horizontal plane
                var headbobOffset = new Vector3(bobAmountX, bobAmountY, 0f);

                // Apply the headbob offset to the camera's local position
                if (_isCameraNotNull)
                    playerCam.transform.localPosition =
                        _originalLocalPosition + headbobOffset * headbobSpeedMultiplier;

                // Increment the headbob timer based on the movement speed
                _headbobTimer += moveSpeed * headbobFrequency * Time.deltaTime;
            }
            else
            {
                // Reset headbob when not moving or in the air
                playerCam.transform.localPosition = Vector3.Lerp(playerCam.transform.localPosition, _originalLocalPosition, Time.deltaTime * headbobSpeedMultiplier);
                _headbobTimer = 0f;
            }
        }
    }
}