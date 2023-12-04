using UnityEngine;

namespace WeaponRelated
{
    public class WeaponSway : MonoBehaviour
    {
        [Header("Sway Rotation")]
        [SerializeField] private float rotationAmount;
        [SerializeField] private float maxRotationAmount;
        [SerializeField] private float smoothRotation;
        [SerializeField] private bool rotationX = true;
        [SerializeField] private bool rotationY = true;
        [SerializeField] private bool rotationZ = true;
        private float _inputX;
        private float _inputY;
        private Quaternion _initialRotation;
    
        private void Awake()
        {
            _initialRotation = transform.localRotation;
        }

        private void Update()
        {
            CalculateSway();
            TiltSway();
        }

        private void CalculateSway()
        {
            _inputX = -Input.GetAxis("Mouse X");
            _inputY = -Input.GetAxis("Mouse Y");
        }

        private void TiltSway()
        {
            // Calculate the tilt sway rotation along the X and Y axes based on mouse input
            var tiltY = Mathf.Clamp(_inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
            var tiltX = Mathf.Clamp(_inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

            // Create a Quaternion representing the final sway rotation
            Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));

            // Smoothly interpolate between the current weapon rotation and the final sway rotation
            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * _initialRotation, Time.deltaTime * smoothRotation);
        }
    }
}
