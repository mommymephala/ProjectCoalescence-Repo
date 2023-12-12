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
            other.transform.position = labTpLocation.position;
            labTeleportToBotanicTrigger.SetActive(false);
        }
    }
}
