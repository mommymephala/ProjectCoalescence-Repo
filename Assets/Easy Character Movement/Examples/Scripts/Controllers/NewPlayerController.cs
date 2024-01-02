using ECM.Common;
using ECM.Controllers;
using UnityEngine;

namespace ECM.Examples
{
    /// <summary>
    /// Headbob animation example:
    ///
    /// This example shows how to create a custom first person controller, this extends the BaseFirstPerson controller
    /// and adds Headbob animation. To do this, we animate the cameraPivot transform, this way we can tailor fit the camera
    /// headbob animation to match our game needs, additionally, we can use Animation events to trigger sound effects like footsteps, etc.
    /// </summary>

    public class NewPlayerController : BaseFirstPersonController
    {
        #region EDITOR EXPOSED FIELDS

        [Header("Headbob")]
        public Animator cameraAnimator;

        public float cameraAnimSpeed = 1.0f;

        #endregion

        #region FIELDS

        private int _verticalParamId;
        private int _horizontalParamId;

        #endregion

        #region METHODS

        /// <summary>
        /// Animate camera pivot to play Headbob like animations, Feed CharacterMovement info to camera animator.
        /// </summary>

        private void AnimateCamera()
        {
            //var normalizedSpeed = Mathf.InverseLerp(0.0f, forwardSpeed, movement.velocity.onlyXZ().magnitude);
            
            var lateralVelocity = Vector3.ProjectOnPlane(movement.velocity, transform.up);
            var normalizedSpeed = Mathf.InverseLerp(0.0f, forwardSpeed, lateralVelocity.magnitude);

            cameraAnimator.speed = Mathf.Max(0.5f, cameraAnimSpeed * normalizedSpeed);

            const float dampTime = 0.1f;

            cameraAnimator.SetFloat(_verticalParamId, moveDirection.z, dampTime, Time.deltaTime);
            cameraAnimator.SetFloat(_horizontalParamId, moveDirection.x, dampTime, Time.deltaTime);
        }

        /// <summary>
        /// Override BaseFirstPersonController Animate method.
        /// </summary>

        protected override void Animate()
        {
            AnimateCamera();
        }

        /// <summary>
        /// Override BaseFirstPersonController HandleInput method.
        /// </summary>

        // protected override void HandleInput()
        // {
        //     // Possible custom input code
        // }

        /// <summary>
        /// Override BaseFirstPersonController Awake method.
        /// </summary>

        public override void Awake()
        {
            // Initialize BaseFirstPersonController

            base.Awake();

            // Cache animator parameter and state ids

            _verticalParamId = Animator.StringToHash("vertical");
            _horizontalParamId = Animator.StringToHash("horizontal");
        }

        #endregion
    }
}
