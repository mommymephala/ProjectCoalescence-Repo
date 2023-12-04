using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField]
    private Door door;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!door.isOpen)
        {
            door.Open(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (door.isOpen)
        {
            door.Close();
        }
    }
}