using ECM.Components;
using ECM.Examples;
using UnityEngine;

public class SeamlessTeleporter : MonoBehaviour
{
    public Transform teleportTarget;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var playerRb = other.GetComponent<Rigidbody>();
            var playerMovement = other.GetComponent<NewPlayerController>();
            var playerController = other.GetComponent<CharacterMovement>();

            if (playerRb != null && playerMovement != null)
            {
                // Disable player movement
                playerController.enabled = false;
                playerMovement.enabled = false;

                // Optionally, disable Rigidbody interpolation
                RigidbodyInterpolation originalInterpolation = playerRb.interpolation;
                playerRb.interpolation = RigidbodyInterpolation.None;

                // Save current velocity and angular velocity
                Vector3 velocity = playerRb.velocity;
                Vector3 angularVelocity = playerRb.angularVelocity;

                // Teleport player
                other.transform.position = teleportTarget.position;

                // Apply saved velocity and angular velocity
                playerRb.velocity = teleportTarget.transform.TransformDirection(velocity);
                playerRb.angularVelocity = angularVelocity;

                // Re-enable interpolation if it was enabled
                playerRb.interpolation = originalInterpolation;

                playerController.enabled = true;
                playerMovement.enabled = true;
            }
        }
    }
}