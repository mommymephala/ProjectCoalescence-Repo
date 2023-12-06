using UI;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using WeaponRelated;

namespace PlayerActions
{
    public class PlayerMovement : MonoBehaviour
    {
        [Header("Inventory")]
        public Canvas InventoryCanvas;
        public bool IsInventoryOpen { get; private set; }
        [SerializeField] private  CursorVisibility cursorVisibility;
        [SerializeField] private  PlayerLook playerLook;
        [SerializeField] private  Weapon deagleweapon;
        [SerializeField] private  Weapon rifleWeapon;
        [SerializeField] private  Weapon shotgunWeapon;
        
         private AudioMenager audioMenager;
        
         private float footstepTimer = 0.25f;
         private const float footstepInternal = 0.70f;
         private float RunfootstepTimer = 0.25f;
         private const float RunfootstepInternal = 0.50f;
         
        private const float PlayerHeight = 2f;
        [SerializeField] private Transform orientation;

        [Header("Movement")]
        private const float MovementMultiplier = 10f;
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

        [Header("Keybinds")]
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        [SerializeField] private KeyCode sprintKey = KeyCode.LeftShift;

        [Header("Drag")]
        [SerializeField] private float groundDrag = 6f;
        [SerializeField] private float airDrag = 2f;
        private float _horizontalMovement;
        private float _verticalMovement;
        private bool _isGrounded;
        private Vector3 _moveDirection;
        private Vector3 _slopeMoveDirection;
        private Rigidbody _rb;
        private RaycastHit _slopeHit;
        

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
            CheckGround();
            TakeInput();
            ControlDrag();
            ControlSpeed();
            
            if (Input.GetKeyDown(jumpKey) && _isGrounded)
            {
                Jump();
            }

            _slopeMoveDirection = Vector3.ProjectOnPlane(_moveDirection, _slopeHit.normal);

            InventoryCanvasCheck();
            if (_isGrounded && !Input.GetKey(sprintKey))
            {
                // Check if there is movement
                bool isMoving = _horizontalMovement != 0f || _verticalMovement != 0f;

                // If there is movement, increment the timer and play footstep sound if enough time has passed
                if (isMoving)
                {
                    footstepTimer +=  Time.deltaTime;
                    if (footstepTimer >= footstepInternal)
                    {
                        audioMenager.PlayFootstep();
                        footstepTimer = 0f; // Reset the timer
                    }
                }
                else
                {
                    // If there is no movement, reset the timer
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
            if (!_isGrounded) return;
            _rb.velocity = new Vector3(_rb.velocity.x, 0, _rb.velocity.z);
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
                    // Check if there is movement
                    bool isMoving = _horizontalMovement != 0f || _verticalMovement != 0f;

                    // If there is movement, increment the timer and play footstep sound if enough time has passed
                    if (isMoving)
                    {
                        RunfootstepTimer +=  Time.deltaTime;
                        if (RunfootstepTimer >= RunfootstepInternal)
                        {
                            audioMenager.PlayFootstep();
                            RunfootstepTimer = 0f; // Reset the timer
                        }
                    }
                    else
                    {
                        // If there is no movement, reset the timer
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
        
        private void CheckGround()
        {
            var origin = new Vector3(transform.position.x, transform.position.y - (transform.localScale.y * .5f), transform.position.z);
            Vector3 direction = transform.TransformDirection(Vector3.down);
            const float distance = .75f;

            if (Physics.Raycast(origin, direction, out RaycastHit _, distance))
            {
                Debug.DrawRay(origin, direction * distance, Color.red);
                _isGrounded = true;
            }
            else
            {
                _isGrounded = false;
            }
        }
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
            deagleweapon.enabled = false;
            rifleWeapon.enabled = false;
            deagleweapon.enabled = false;
            Time.timeScale = 0f;
        }
        public void InventoryCanvasCheckClose()
        {
            InventoryCanvas.enabled = false;
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
            IsInventoryOpen = false;
            playerLook.enabled = true;
            deagleweapon.enabled = true;
            rifleWeapon.enabled = true;
            shotgunWeapon.enabled = true;
            Time.timeScale = 1f;
        }
    }
}