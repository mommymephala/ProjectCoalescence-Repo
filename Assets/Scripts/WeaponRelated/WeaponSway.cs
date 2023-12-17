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
            var tiltY = Mathf.Clamp(_inputX * rotationAmount, -maxRotationAmount, maxRotationAmount);
            var tiltX = Mathf.Clamp(_inputY * rotationAmount, -maxRotationAmount, maxRotationAmount);

            Quaternion finalRotation = Quaternion.Euler(new Vector3(rotationX ? -tiltX : 0f, rotationY ? tiltY : 0f, rotationZ ? tiltY : 0f));

            transform.localRotation = Quaternion.Slerp(transform.localRotation, finalRotation * _initialRotation, Time.deltaTime * smoothRotation);
        }
    }
}
