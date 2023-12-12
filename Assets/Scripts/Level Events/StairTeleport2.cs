using UnityEngine;

public class StairTeleport2 : MonoBehaviour
{
    public Transform stairTeleportLocation2;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            other.transform.position = stairTeleportLocation2.position;
        }
    }
}