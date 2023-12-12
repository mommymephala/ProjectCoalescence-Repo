using UnityEngine;

public class LabLoopTrigger : MonoBehaviour
{
    public GameObject labTeleportToBotanicTrigger;

    //When triggered, turn on lab teleport trigger
    //Always active
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            labTeleportToBotanicTrigger.SetActive(true);
        }
    }
}
