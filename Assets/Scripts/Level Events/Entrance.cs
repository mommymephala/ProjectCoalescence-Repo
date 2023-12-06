using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entrance : MonoBehaviour
{
    public GameObject FirstFloor;

   

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
        FirstFloor.SetActive(false);
        
    }
}
