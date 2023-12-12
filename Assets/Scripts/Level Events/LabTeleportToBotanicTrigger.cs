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
            other.transform.position = botanicTpLocation.position;
            botanicTeleportToLabTrigger.SetActive(false);
        }
    }
}
