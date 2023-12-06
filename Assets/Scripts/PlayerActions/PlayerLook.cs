using Interfaces;
using UnityEngine;

namespace PlayerActions
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Transform camHolder;

        [SerializeField] private Transform orientation;

        [Header("Mouse Sensitivity")] [SerializeField]
        private float mouseSens = 100f;

        [SerializeField] private float adsSens = 50f;

        public InventoryTest inventory;

        public float MaxDistance;
        //Mouse look variables
        private const float Multiplier = 0.01f;
        private float _mouseX;
        private float _mouseY;
        private float _xRotation;
        private float _yRotation;
        private bool _aimingDownSight;

        private void Update()
        {
            HandleMouseLook();
            
            if (Input.GetKeyDown(KeyCode.E))
            {
                PickObject();
            }
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
        public void SetAimingDownSight(bool aiming)
        {
            _aimingDownSight = aiming;
        }
        public void PickObject()
        {
            Ray ray = new Ray(camHolder.transform.position, camHolder.transform.forward);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, MaxDistance))
            {
                IPickUp objectPickable = hit.collider.GetComponent<IPickUp>();
                if (objectPickable != null)
                {
                    inventory.AddItem(hit.collider.GetComponent<Item>());
                    objectPickable.PickUp();
                
                }
            }

        }
    }
}
