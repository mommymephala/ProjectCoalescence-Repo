using Interfaces;
using UnityEngine;

namespace PlayerActions
{
    public class PlayerLook : MonoBehaviour
    {
        [Header("References")] 
        public InventoryTest inventory;
        public float maxDistance;
        [SerializeField] private Transform orientation;
        [SerializeField] private Transform camHolder;

        [Header("Mouse Sensitivity")] 
        [SerializeField] private float mouseSens = 100f;
        [SerializeField] private float adsSens = 50f;
        
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
            _mouseX = Input.GetAxisRaw("Mouse X");
            _mouseY = Input.GetAxisRaw("Mouse Y");

            var currentMouseSensitivity = _aimingDownSight ? adsSens : mouseSens;

            _yRotation += _mouseX * currentMouseSensitivity * Multiplier;
            _xRotation -= _mouseY * currentMouseSensitivity * Multiplier;

            _xRotation = Mathf.Clamp(_xRotation, -90f, 90f);

            camHolder.transform.rotation = Quaternion.Euler(_xRotation, _yRotation, 0);
            orientation.transform.rotation = Quaternion.Euler(0, _yRotation, 0);
        }
        public void SetAimingDownSight(bool aiming)
        {
            _aimingDownSight = aiming;
        }

        private void PickObject()
        {
            var ray = new Ray(camHolder.transform.position, camHolder.transform.forward);

            if (Physics.Raycast(ray, out RaycastHit hit, maxDistance))
            {
                var objectPickable = hit.collider.GetComponent<IPickUp>();
                if (objectPickable != null)
                {
                    inventory.AddItem(hit.collider.GetComponent<Item>());
                    objectPickable.PickUp();
                }
            }
        }
    }
}
