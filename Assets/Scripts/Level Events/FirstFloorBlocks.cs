using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstFloorBlocks : MonoBehaviour
{
    
    public GameObject FirstFloorBlockObject;


    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player")) return;
      
        FirstFloorBlockObject.SetActive(true);
    }
}
