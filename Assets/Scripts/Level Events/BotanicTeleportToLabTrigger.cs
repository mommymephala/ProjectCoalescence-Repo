using UnityEngine;

public class BotanicTeleportToLabTrigger : MonoBehaviour
{
    public Transform LabTpLocation;
    public GameObject LabTeleportToBotanicTrigger;

    //When triggered, teleport to LabTpLocation
    //Turn off when player is teleported to BotanicTpLocation, in other words, when lab teleport trigger is triggered
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = LabTpLocation.position;
            LabTeleportToBotanicTrigger.SetActive(false);
        }
    }
}
