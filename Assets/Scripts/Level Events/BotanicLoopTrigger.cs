using UnityEngine;

public class BotanicLoopTrigger : MonoBehaviour
{
    public GameObject BotanicTeleportToLabTrigger;

    //When triggered, turn on botanic teleport trigger
    //Always active
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            BotanicTeleportToLabTrigger.SetActive(true);
        }
    }
}
