using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class Kurt_Player : MonoBehaviour
{

    private float speed = 5;

    public InventoryTest inventory;
    
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
    }

    private void HandleMovement()
    {
        float translation = speed * Time.deltaTime;
        
        
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * translation,0,Input.GetAxis("Vertical") * translation));
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            inventory.AddItem(other.GetComponent<Item>());
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            inventory.AddItem(collision.gameObject.GetComponent<Item>());
            Destroy(collision.gameObject);
        }
    }
}
