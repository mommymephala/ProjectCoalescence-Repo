using ECM.Components;
using ECM.Examples;
using UnityEngine;

namespace Level_Events
{
    public class TeleportationManager : MonoBehaviour
    {
        public static TeleportationManager Instance { get; private set; }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        
        public static void TeleportPlayer(Transform playerTransform, Transform targetTransform, Rigidbody playerRb, NewPlayerController playerMovement, CharacterMovement playerController)
        {
            playerController.enabled = false;
            playerMovement.enabled = false;

            RigidbodyInterpolation originalInterpolation = playerRb.interpolation;
            playerRb.interpolation = RigidbodyInterpolation.None;

            Vector3 velocity = playerRb.velocity;
            Vector3 angularVelocity = playerRb.angularVelocity;

            playerTransform.position = targetTransform.position;

            playerRb.velocity = targetTransform.TransformDirection(velocity);
            playerRb.angularVelocity = angularVelocity;

            playerRb.interpolation = originalInterpolation;

            playerController.enabled = true;
            playerMovement.enabled = true;
        }
    }
}