using System;
using System.Collections;
using System.Collections.Generic;
using Interfaces;
using Unity.VisualScripting;
using UnityEngine;

public class Kurt_Player : MonoBehaviour
{

    private float speed = 5;

    public float maxDistance = 5;
    

    public InventoryTest inventory;
    private IPickUp pickUpImplementation;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();

        if (Input.GetKeyDown(KeyCode.E))
        {
            PickObject();
        }
    }

    private void HandleMovement()
    {
        float translation = speed * Time.deltaTime;
        
        
        transform.Translate(new Vector3(Input.GetAxis("Horizontal") * translation,0,Input.GetAxis("Vertical") * translation));
    }

  /* private void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Item")
        {
            inventory.AddItem(other.GetComponent<Item>());
        }
    }*/

   /* private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Item")
        {
            //inventory.AddItem(collision.gameObject.GetComponent<Item>());
            Destroy(collision.gameObject);
        }
    }*/
    
    public void PickObject()
    {
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, maxDistance))
        {
            IPickUp objectPickable = hit.collider.GetComponent<IPickUp>();
            if (objectPickable != null)
            {
                inventory.AddItem(hit.collider.GetComponent<Item>());
                objectPickable.PickUp();
                
            }
        }

    }
}
