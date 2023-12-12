using UnityEngine;

public class Entrance : MonoBehaviour
{
    public GameObject firstFloor;
    
    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        firstFloor.SetActive(false);
        
    }
}
