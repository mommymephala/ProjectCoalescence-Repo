using UnityEngine;
using WeaponRelated;

namespace PlayerActions
{
    public class PlayerMovement : MonoBehaviour
    {
        // Re-do and separate
        [Header("Inventory")]
        public Canvas inventoryCanvas;
        public DynamicCrosshair crosshair;
        [SerializeField] private PlayerLook playerLook;
        [SerializeField] private Weapon deagleWeapon;
        [SerializeField] private Weapon rifleWeapon;
        [SerializeField] private Weapon shotgunWeapon;
        public bool IsInventoryOpen { get; private set; }
        
        private AudioManager _audioManager;
        private float _footstepTimer = 0.25f;
        private const float FootstepInternal = 0.70f;
        private float _runfootstepTimer = 0.25f;
        private const float RunfootstepInternal = 0.50f;

        [Header("Movement")]
        private const float PlayerHeight = 2f;
        private const float MovementMultiplier = 10f;
        [SerializeField] private Transform orientation;
        [SerializeField] public float moveSpeed = 1f;
        [SerializeField] private float airMultiplier = 1f;
        [SerializeField] private Vector3 gravity;
        
        public bool IsSprinting { get; private set; }
        public bool IsWalking { get; private set; }

        [Header("Sprinting")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] public float sprintSpeed = 6f;
        [SerializeField] private float acceleration = 10f;

        [Header("Jumping")]
        public float jumpForce = 5f;
        
        // Slope Handling
        private RaycastHit _slopeHit;
        private Vector3 _slopeMoveDirection;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Drag")]
        [SerializeField] private float groundDrag = 6f;
        [SerializeField] private float airDrag = 2f;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask groundMask;
        [SerializeField] private float groundDistance = 0.2f;
        private bool _isGrounded;
        
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _moveDirection;
        private Rigidbody _rb;
        
        /*
         *
         *
         *
         *
         *
         * DO BETTER SOUND IMPLEMENTATION, SEPARATE INVENTORY LOGIC
         *
         *
         *
         *
         * 
         */

        private void Awake()
        {
            _audioManager = FindObjectOfType<AudioManager>();
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }

        private void Update()
        {
            InventoryCanvasCheck();
            CheckGround();
            TakeInput();
            ControlDrag();
            ControlSpeed();
            
            if (Input.GetKeyDown(KeyCode.Alpha4))
            {
                crosshair.gameObject.SetActive(true);
            }
            
            if (Input.GetKeyDown(jumpKey) && _isGrounded)
            {
                Jump();
            }

            if (!_isGrounded || Input.GetKey(sprintKey)) return;
            if (IsMoving())
            {
                _footstepTimer +=  Time.deltaTime;
                if (_footstepTimer >= FootstepInternal)
                {
                    _audioManager.PlayFootstep();
                    _footstepTimer = 0f;
                }
            }
            else
            {
                _footstepTimer = FootstepInternal;
            }
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
        
        private void Jump()
        {
            _rb.velocity = new Vector3(_rb.velocity.x, 0f, _rb.velocity.z);
            _rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        }

        private void ControlSpeed()
        {
            if (Input.GetKey(sprintKey) && _isGrounded)
            {
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
                IsSprinting = true;
                if (!_isGrounded) return;
                if (IsMoving())
                {
                    _runfootstepTimer +=  Time.deltaTime;
                    if (_runfootstepTimer >= RunfootstepInternal)
                    {
                        _audioManager.PlayFootstep();
                        _runfootstepTimer = 0f;
                    }
                }
                else
                {
                    _runfootstepTimer = RunfootstepInternal;
                }
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
            if (Physics.Raycast(transform.position, Vector3.down, out _slopeHit, PlayerHeight / 2 + 0.5f))
            {
                if (_slopeHit.normal != Vector3.up)
                {
                    return true;
                }

                return false;
            }
            return false;
        }
        
        private void MovePlayer()
        {
            if (_isGrounded)
            {
                if (OnSlope())
                {
                    HandleSlopeMovement();
                }
                else
                {
                    // Regular grounded movement
                    _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
                }
            }
            else
            {
                // Apply force in the air
                _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier * airMultiplier), ForceMode.Acceleration);
            }
        }

        private void HandleSlopeMovement()
        {
            _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;

            // Apply force along the slope's direction
            _rb.AddForce(_slopeMoveDirection * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);

            // Additional logic to prevent sliding down slopes
            if (!IsMoving())
            {
                _rb.AddForce(-_slopeHit.normal * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
            }
        }

        private void ApplyGravity()
        {
            if (!_isGrounded)
            {
                _rb.AddForce(gravity, ForceMode.Acceleration);
            }
        }

        private void CheckGround()
        {
            var isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask); // Initialize the flag to false
            _isGrounded = isGrounded;

            // DebugCheckGround();
        }

        private bool IsMoving()
        {
            return _horizontalMovement != 0f || _verticalMovement != 0f;
        }
        
        private void DebugCheckGround()
        {
            // Set the color for the debug sphere
            Color debugColor = _isGrounded ? Color.green : Color.red;

            // Draw a visible debug sphere at the ground check position
            Debug.DrawRay(groundCheck.position, Vector3.down * groundDistance, debugColor);
            DrawWireSphere(groundCheck.position, groundDistance, debugColor);
        }
        
        private static void DrawWireSphere(Vector3 position, float radius, Color color)
        {
            int segments = 36; // Number of line segments to create a circle
            float angleIncrement = 360f / segments;
            Vector3 prevPoint = Vector3.zero;

            for (int i = 0; i <= segments; i++)
            {
                float angle = i * angleIncrement;
                float x = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;
                float z = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
                Vector3 currentPoint = position + new Vector3(x, 0f, z);

                if (i > 0)
                {
                    Debug.DrawLine(prevPoint, currentPoint, color);
                }

                prevPoint = currentPoint;
            }
        }
        
        // ################################################################################################################ //
        
        public void InventoryCanvasCheck()
        {
            if (Input.GetKeyDown(KeyCode.Tab) && (IsInventoryOpen))
            {
               InventoryCanvasCheckClose();
            }
            else if (Input.GetKeyDown(KeyCode.Tab) && (!IsInventoryOpen))
            {
                InventoryCanvasCheckOpen();
            }
        }
        public void InventoryCanvasCheckOpen()
        {
            inventoryCanvas.enabled = true;
            IsInventoryOpen = true;
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.Confined;
            playerLook.enabled = false;
            deagleWeapon.enabled = false;
            rifleWeapon.enabled = false;
            deagleWeapon.enabled = false;
            Time.timeScale = 0f;
        }
        public void InventoryCanvasCheckClose()
        {
            inventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsInventoryOpen = false;
            playerLook.enabled = true;
            deagleWeapon.enabled = true;
            rifleWeapon.enabled = true;
            shotgunWeapon.enabled = true;
            Time.timeScale = 1f;
        }
        
        // ################################################################################################################ //
        
    }
}