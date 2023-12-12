using UnityEngine;

public class StairTeleport : MonoBehaviour
{
    public Transform stairTeleportLocation;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = stairTeleportLocation.position;
        }
    }
}