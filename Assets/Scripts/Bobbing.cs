using System;
using ECM.Components;
using ECM.Controllers;
using ECM.Examples;
using UnityEngine;

namespace PlayerActions
{
    public class Bobbing : MonoBehaviour
    {
        [Header("Bob")]
        [SerializeField] private bool toggleBob = true;
        [SerializeField] private Transform joint;
        [SerializeField] private float bobSpeed = 10f;
        [SerializeField] private Vector3 bobAmount = new Vector3(.15f, .05f, 0f);
        
        // Bobbing values when aiming
        [SerializeField] private float aimingBobSpeed = 5f;
        [SerializeField] private Vector3 aimingBobAmount = new Vector3(.05f, .02f, 0f);
        
        // References
        private float _bobTimer;
        private Vector3 _jointOriginalPos;
        private NewPlayerController _newPlayerController;
        private MouseLook _mouseLook;

        private void Awake()
        {
            _newPlayerController = FindObjectOfType<NewPlayerController>();
            _mouseLook = GetComponentInParent<MouseLook>();
        }

        private void Update()
        {
            ApplyBobbing();
        }

        private void ApplyBobbing()
        {
            if (!toggleBob) return;

            // Check if the player is aiming
            bool isAiming = _mouseLook.aimingDownSight;

            float currentBobSpeed = bobSpeed;
            Vector3 currentBobAmount = bobAmount;

            if (isAiming)
            {
                currentBobSpeed = aimingBobSpeed;
                currentBobAmount = aimingBobAmount;
            }
            else if (_newPlayerController.GetTargetSpeed() > 0)
            {
                currentBobSpeed = _newPlayerController.run ? bobSpeed + _newPlayerController.runSpeedMultiplier * _newPlayerController.GetTargetSpeed() * 0.5f : bobSpeed;
            }
            else
            {
                _bobTimer = 0;
                joint.localPosition = Vector3.Lerp(joint.localPosition, _jointOriginalPos, Time.deltaTime * currentBobSpeed);
                return;
            }

            _bobTimer += Time.deltaTime * currentBobSpeed;

            joint.localPosition = new Vector3(
                _jointOriginalPos.x + Mathf.Sin(_bobTimer) * currentBobAmount.x,
                _jointOriginalPos.y + Mathf.Sin(_bobTimer) * currentBobAmount.y,
                _jointOriginalPos.z + Mathf.Sin(_bobTimer) * currentBobAmount.z
            );
        }
    }
}