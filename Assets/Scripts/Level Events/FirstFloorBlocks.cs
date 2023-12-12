using UnityEngine;

public class FirstFloorBlocks : MonoBehaviour
{
    public GameObject firstFloorBlockObject;

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
      
        firstFloorBlockObject.SetActive(true);
    }
}
