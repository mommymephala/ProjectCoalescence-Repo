using UnityEngine;

public class StairTeleport : MonoBehaviour
{
    public Transform StairTeleportLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = StairTeleportLocation.position;
        }
    }
}