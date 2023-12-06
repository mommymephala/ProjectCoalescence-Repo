using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StairTrigger : MonoBehaviour
{
    public GameObject FirstFloor;
    public GameObject FirstFloorBlock;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        FirstFloor.SetActive(true);
        FirstFloorBlock.SetActive(false);
    }
}
