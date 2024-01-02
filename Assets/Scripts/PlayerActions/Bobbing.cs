using System;
using ECM.Controllers;
using ECM.Examples;
using UnityEngine;

namespace PlayerActions
{
    public class Bobbing : MonoBehaviour
    {
        [Header("Headbob")]
        [SerializeField] private bool toggleHeadbob = true;
        [SerializeField] private Transform joint;
        [SerializeField] private float bobSpeed = 10f;
        [SerializeField] private Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
        
        // References
        private float _headbobTimer;
        private Vector3 _jointOriginalPos;
        private NewPlayerController _newPlayerController;

        private void Awake()
        {
            _newPlayerController = FindObjectOfType<NewPlayerController>();
        }

        private void Update()
        {
            ApplyBobbing();
        }

        private void ApplyBobbing()
        {
            if (!toggleHeadbob) return;

            if (_newPlayerController.GetTargetSpeed() > 0)
            {
                var currentBobSpeed = _newPlayerController.run ? (bobSpeed + (_newPlayerController.runSpeedMultiplier * _newPlayerController.GetTargetSpeed() * 0.5f)) : bobSpeed;
        
                _headbobTimer += Time.deltaTime * currentBobSpeed;

                joint.localPosition = new Vector3(
                    _jointOriginalPos.x + Mathf.Sin(_headbobTimer) * bobAmount.x,
                    _jointOriginalPos.y + Mathf.Sin(_headbobTimer) * bobAmount.y,
                    _jointOriginalPos.z + Mathf.Sin(_headbobTimer) * bobAmount.z
                );
            }
            else
            {
                _headbobTimer = 0;

                joint.localPosition = Vector3.Lerp
                (
                    joint.localPosition,
                    _jointOriginalPos,
                    Time.deltaTime * bobSpeed
                );
            }
        }
    }
}
