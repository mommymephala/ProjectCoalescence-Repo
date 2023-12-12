using UnityEngine;
using WeaponRelated;

namespace PlayerActions
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Inventory")]
        public Canvas InventoryCanvas;
        public DynamicCrosshair crosshair;
        
        [SerializeField] private  PlayerLook playerLook;
        [SerializeField] private  Weapon deagleWeapon;
        [SerializeField] private  Weapon rifleWeapon;
        [SerializeField] private  Weapon shotgunWeapon;
        private AudioMenager audioMenager;
        public bool IsInventoryOpen { get; private set; }
        
        private float footstepTimer = 0.25f;
        private const float footstepInternal = 0.70f;
        private float RunfootstepTimer = 0.25f;
        private const float RunfootstepInternal = 0.50f;

        [Header("Movement")]
        private const float PlayerHeight = 2f;
        private const float MovementMultiplier = 10f;
        [SerializeField] private Transform orientation;
        [SerializeField] public float moveSpeed = 1f;
        [SerializeField] private float airMultiplier = 1f;
        [SerializeField] private Vector3 gravity;

        [Header("Sprinting")]
        [SerializeField] private float walkSpeed = 4f;
        [SerializeField] public float sprintSpeed = 6f;
        [SerializeField] private float acceleration = 10f;
        public bool IsSprinting { get; private set; }
        public bool IsWalking { get; private set; }

        [Header("Jumping")]
        public float jumpForce = 5f;
        
        [Header("Slope Handling")]
        // [SerializeField] private float maxSlopeAngle;
        private RaycastHit _slopeHit;
        // private bool _exitingSlope;
        private Vector3 _slopeMoveDirection;

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Drag")]
        [SerializeField] private float groundDrag = 6f;
        [SerializeField] private float airDrag = 2f;
        
        [Header("Ground Detection")]
        [SerializeField] private Transform groundCheck;
        [SerializeField] private LayerMask[] groundMask;
        [SerializeField] private float groundDistance = 0.2f;
        private bool _isGrounded;
        
        private float _horizontalMovement;
        private float _verticalMovement;
        private Vector3 _moveDirection;
        private Rigidbody _rb;

        private void Awake()
        {
            _rb = GetComponent<Rigidbody>();
            _rb.freezeRotation = true;
        }
        private void Start()
        {
            audioMenager = FindObjectOfType<AudioMenager>();
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

            // _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);
            
            if (_isGrounded && !Input.GetKey(sprintKey))
            {
                bool isMoving = _horizontalMovement != 0f || _verticalMovement != 0f;

                if (isMoving)
                {
                    footstepTimer +=  Time.deltaTime;
                    if (footstepTimer >= footstepInternal)
                    {
                        audioMenager.PlayFootstep();
                        footstepTimer = 0f;
                    }
                }
                else
                {
                    footstepTimer = footstepInternal;
                }
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
                if (_isGrounded)
                {
                    bool isMoving = _horizontalMovement != 0f || _verticalMovement != 0f;

                    if (isMoving)
                    {
                        RunfootstepTimer +=  Time.deltaTime;
                        if (RunfootstepTimer >= RunfootstepInternal)
                        {
                            audioMenager.PlayFootstep();
                            RunfootstepTimer = 0f;
                        }
                    }
                    else
                    {
                        RunfootstepTimer = RunfootstepInternal;
                    }
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
            if (!(_horizontalMovement != 0f || _verticalMovement != 0f))
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
            bool isGrounded = false; // Initialize the flag to false

            foreach (LayerMask layerMask in groundMask)
            {
                if (Physics.CheckSphere(groundCheck.position, groundDistance, layerMask))
                {
                    isGrounded = true;
                    break;
                }
            }

            _isGrounded = isGrounded;

            DebugCheckGround();
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
        
        // ################################################################################################################
        
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
            InventoryCanvas.enabled = true;
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
            InventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsInventoryOpen = false;
            playerLook.enabled = true;
            deagleWeapon.enabled = true;
            rifleWeapon.enabled = true;
            shotgunWeapon.enabled = true;
            Time.timeScale = 1f;
        }
        
        // ################################################################################################################
        
        /*private bool CheckForStep(out Vector3 stepUpOffset)
        {
            stepUpOffset = Vector3.zero;
            RaycastHit hit;

            // New step detection logic
            Vector3 rayStart = transform.position + (transform.forward * 0.1f) + (Vector3.up * maxStepHeight);
            Vector3 rayDirection = Vector3.down;

            if (Physics.Raycast(rayStart, rayDirection, out hit, maxStepHeight))
            {
                if (hit.point.y > transform.position.y && hit.point.y < transform.position.y + maxStepHeight)
                {
                    stepUpOffset = new Vector3(0f, maxStepHeight - (rayStart.y - hit.point.y), 0f);
                    return true;
                }
            }
            return false;
        }*/

        /*private void MovePlayer()
        {
            // Check if there's a slope
            bool onSlope = OnSlope();

            if (_isGrounded && !onSlope)
            {
                _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
            }
            else if (onSlope && !_exitingSlope)
            {
                _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal).normalized;
                _rb.AddForce(_slopeMoveDirection * (moveSpeed * MovementMultiplier), ForceMode.Acceleration);
            }
            else
            {
                _rb.AddForce(_moveDirection.normalized * (moveSpeed * MovementMultiplier * airMultiplier), ForceMode.Acceleration);
            }

            // Enhanced Step Logic
            if (CheckForStep(out Vector3 stepUpOffset))
            {
                _rb.position += stepUpOffset; // Step up
            }
        }*/
        
        /*private void CheckGround()
        {
            float colliderRadius = 0.5f; // Adjust as necessary
            float colliderHeight = PlayerHeight;
            Vector3 boxCenter = transform.position + (Vector3.down * (colliderHeight / 2 - colliderRadius / 2));
            Vector3 halfExtents = new Vector3(colliderRadius, 0.1f, colliderRadius);
            Quaternion orientation = Quaternion.identity;
            float maxDistance = 0.2f; // Adjust for better ground detection on slopes

            if (Physics.BoxCast(boxCenter, halfExtents, Vector3.down, out RaycastHit hit, orientation, maxDistance))
            {
                if (Vector3.Angle(Vector3.up, hit.normal) <= maxSlopeAngle)
                {
                    _isGrounded = true;
                }
                else
                {
                    _isGrounded = false;
                }
                if (Vector3.Angle(Vector3.up, _slopeHit.normal) <= maxSlopeAngle)
                {
                    _isGrounded = true;
                }
                else
                {
                    _isGrounded = false;
                }
            }
            else
            {
                _isGrounded = false;
            }

            // Debug visualization
            DebugDrawBox(boxCenter, halfExtents, orientation, maxDistance, _isGrounded ? Color.green : Color.red);
        }*/

        /*private void DebugDrawBox(Vector3 center, Vector3 halfExtents, Quaternion orientation, float maxDistance, Color color)
        {
            Vector3 up = orientation * Vector3.up;
            Vector3 right = orientation * Vector3.right;
            Vector3 forward = orientation * Vector3.forward;

            Vector3 p1 = center + up * halfExtents.y - right * halfExtents.x - forward * halfExtents.z;
            Vector3 p2 = center + up * halfExtents.y + right * halfExtents.x - forward * halfExtents.z;
            Vector3 p3 = center + up * halfExtents.y - right * halfExtents.x + forward * halfExtents.z;
            Vector3 p4 = center + up * halfExtents.y + right * halfExtents.x + forward * halfExtents.z;
            Vector3 p5 = center - up * halfExtents.y - right * halfExtents.x - forward * halfExtents.z;
            Vector3 p6 = center - up * halfExtents.y + right * halfExtents.x - forward * halfExtents.z;
            Vector3 p7 = center - up * halfExtents.y - right * halfExtents.x + forward * halfExtents.z;
            Vector3 p8 = center - up * halfExtents.y + right * halfExtents.x + forward * halfExtents.z;

            // Draw the lines considering the maxDistance
            Debug.DrawLine(p1, p2, color, maxDistance);
            Debug.DrawLine(p2, p4, color, maxDistance);
            Debug.DrawLine(p4, p3, color, maxDistance);
            Debug.DrawLine(p3, p1, color, maxDistance);
            Debug.DrawLine(p5, p6, color, maxDistance);
            Debug.DrawLine(p6, p8, color, maxDistance);
            Debug.DrawLine(p8, p7, color, maxDistance);
            Debug.DrawLine(p7, p5, color, maxDistance);
            Debug.DrawLine(p1, p5, color, maxDistance);
            Debug.DrawLine(p2, p6, color, maxDistance);
            Debug.DrawLine(p4, p8, color, maxDistance);
            Debug.DrawLine(p3, p7, color, maxDistance);
        }*/
    }
}