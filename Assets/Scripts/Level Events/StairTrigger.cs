using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    public GameObject firstFloor;
    public GameObject firstFloorBlock;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        firstFloor.SetActive(true);
        firstFloorBlock.SetActive(false);
    }
}
