using UnityEngine;

namespace PlayerActions
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Transform camHolder;
        [SerializeField] private Transform orientation;
        [SerializeField] private GameObject head;

        [Header("Mouse Sensitivity")]
        [SerializeField] private float mouseSens = 100f;
        [SerializeField] private float adsSens = 50f;

        [Header("Leaning")]
        [SerializeField] private float leanAngle = 15f;
        [SerializeField] private KeyCode leanLeftKey = KeyCode.Q;
        [SerializeField] private KeyCode leanRightKey = KeyCode.E;
        [SerializeField] private float leanSmoothTime = 0.1f;
        private bool _isLeaning;
        private float _targetLeanAngle;
        private float _currentLeanAngle;
        private int _leaningDirection; // 0 for no leaning, -1 for left, 1 for right

        //Mouse look variables
        private const float Multiplier = 0.01f;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private float _yRotation;
        private bool _aimingDownSight;

        private void Start()
        {
            head.gameObject.SetActive(false);
        }

        private void Update()
        {
            HandleMouseLook();
            HandleLeaning();
        }

        private void HandleMouseLook()
        {
            // Get mouse input
            _mouseX = Input.GetAxisRaw("Mouse X");
            _mouseY = Input.GetAxisRaw("Mouse Y");

            // Determine current mouse sensitivity
            var currentMouseSensitivity = _aimingDownSight ? adsSens : mouseSens;

            // Apply mouse input to rotation variables
            _yRotation += _mouseX * currentMouseSensitivity * Multiplier;
            _xRotation -= _mouseY * currentMouseSensitivity * Multiplier;

            // Clamp the X rotation to avoid over-rotation
            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            // Update camera and orientation rotations
            camHolder.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            orientation.transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
            
        //Delete it for the other game.
        private void HandleLeaning()
        {
            HandleLeanInput();
            // Smoothly lerp the current lean angle to the target lean angle
            _currentLeanAngle = Mathf.Lerp(_currentLeanAngle, _isLeaning ? _targetLeanAngle : 0f, Time.deltaTime / leanSmoothTime);
            // Set the camera rotation using the current lean angle, preserving the X and Y rotations-
            camHolder.localRotation = Quaternion.Euler(camHolder.localRotation.eulerAngles.x, camHolder.localRotation.eulerAngles.y, _currentLeanAngle);
        }

        private void HandleLeanInput()
        {
            // Check for leaning left input
            if (Input.GetKeyDown(leanLeftKey))
            {
                if (_leaningDirection == -1)
                {
                    _isLeaning = false;
                    head.SetActive(false);
                    _leaningDirection = 0;
                    _targetLeanAngle = 0f;
                }
                else
                {
                    _isLeaning = true;
                    head.SetActive(true);
                    _leaningDirection = -1;
                    _targetLeanAngle = leanAngle;
                }
            }

            // Check for leaning right input
            if (!Input.GetKeyDown(leanRightKey)) return;
            if (_leaningDirection == 1)
            {
                _isLeaning = false;
                head.SetActive(false);
                _leaningDirection = 0;
                _targetLeanAngle = 0f;
            }
            else
            {
                _isLeaning = true;
                head.SetActive(true);
                _leaningDirection = 1;
                _targetLeanAngle = -leanAngle;
            }
        }

        public void SetAimingDownSight(bool aiming)
        {
            _aimingDownSight = aiming;
        }
    }
}
