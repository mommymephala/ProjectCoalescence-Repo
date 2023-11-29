using UnityEngine;

public class LabLoopTrigger : MonoBehaviour
{
    public GameObject LabTeleportToBotanicTrigger;

    //When triggered, turn on lab teleport trigger
    //Always active
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            LabTeleportToBotanicTrigger.SetActive(true);
        }
    }
}
