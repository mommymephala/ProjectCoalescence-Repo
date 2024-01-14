using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [SerializeField] private DoorOpening doorOpening;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (!doorOpening.isOpen)
        {
            doorOpening.Open(other.transform.position);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        if (doorOpening.isOpen)
        {
            doorOpening.Close();
        }
    }
}