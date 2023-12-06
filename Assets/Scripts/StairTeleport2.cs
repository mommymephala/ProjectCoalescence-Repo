using UnityEngine;

public class StairTeleport2 : MonoBehaviour
{
    public Transform StairTeleportLocation2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = StairTeleportLocation2.position;
        }
    }
}