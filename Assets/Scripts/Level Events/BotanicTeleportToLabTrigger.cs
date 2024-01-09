using UnityEngine;

public class BotanicTeleportToLabTrigger : MonoBehaviour
{
    public Transform labTpLocation;
    public GameObject labTeleportToBotanicTrigger;

    //When triggered, teleport to LabTpLocation
    //Turn off when player is teleported to BotanicTpLocation, in other words, when lab teleport trigger is triggered
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // other.transform.position = teleportTarget.position;
            var playerRb = other.GetComponent<Rigidbody>();

            if (playerRb != null)
            {
                // Optionally, disable Rigidbody interpolation
                //var originalInterpolation = playerRb.interpolation;
                //playerRb.interpolation = RigidbodyInterpolation.None;
                
                // Saving current velocity and angular velocity
                Vector3 velocity = playerRb.velocity;
                Vector3 angularVelocity = playerRb.angularVelocity;

                // Teleporting player
                other.transform.position = labTpLocation.position;
                // other.transform.rotation = teleportTarget.rotation;

                // Applying saved velocity and angular velocity
                playerRb.velocity = labTpLocation.transform.TransformDirection(velocity);
                playerRb.angularVelocity = angularVelocity;
                
                // Re-enable interpolation if it was enabled
                //playerRb.interpolation = originalInterpolation;
                
                labTeleportToBotanicTrigger.gameObject.SetActive(false);
            }
        }
    }
}
