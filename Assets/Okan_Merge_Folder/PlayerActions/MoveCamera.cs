using UnityEngine;

namespace PlayerActions
{
    public class MoveCamera : MonoBehaviour
    {
        [SerializeField] private Transform cameraPosition;
        private bool _iscameraPositionNotNull;

        private void Awake()
        {
            _iscameraPositionNotNull = cameraPosition != null;
        }

        private void Update()
        {
            if (_iscameraPositionNotNull)
            {
                transform.position = cameraPosition.position;
            }
        }
    }
}
