using UnityEngine;

public class LabTeleportToBotanicTrigger : MonoBehaviour
{
    public Transform botanicTpLocation;
    public GameObject botanicTeleportToLabTrigger;
    
    //When triggered, teleport to BotanicTpLocation
    //Turn off when player is teleported to LabTpLocation, in other words, when botanic teleport trigger is triggered
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
                other.transform.position = botanicTpLocation.position;
                // other.transform.rotation = teleportTarget.rotation;

                // Applying saved velocity and angular velocity
                playerRb.velocity = botanicTpLocation.transform.TransformDirection(velocity);
                playerRb.angularVelocity = angularVelocity;
                
                // Re-enable interpolation if it was enabled
                //playerRb.interpolation = originalInterpolation;
                
                botanicTeleportToLabTrigger.gameObject.SetActive(false);
            }
        }
    }
}
