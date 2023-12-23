using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using Interfaces;
using UnityEditor.Scripting;
using UnityEngine;
using UnityEngine.Serialization;

public class Item : MonoBehaviour, IPickUp
{
    [FormerlySerializedAs("ItemData")] public ItemData ıtemData;
    
   // public ItemData type;

  // public ItemType? combinableWith;

    public Sprite spriteNeutral;

    public Sprite spriteHighlighted;
    
    public int maxSize;
    private bool IsCombinable;
    
    public void SetItemType(ItemData newType)
    {
        ıtemData = newType;
        // Handle any updates that need to happen when the item type changes
    }
    private void OnItemTypeChanged()
    {
        // Logic to handle changes when item type is updated
        // For example, update the item's visual representation based on the new type
        UpdateVisualsBasedOnType();
    }
    private void UpdateVisualsBasedOnType()
    {
        // Implementation depends on your game's logic
        // Example: Update sprite or other properties based on the itemType
        // spriteNeutral = itemType.someSprite; // Example usage
    }
    public void Use()

    {
        switch (ıtemData.typeName)
        {
             case "Health":
                // Logic for Health item
                Debug.Log("Health Used");
                break;
             case "Mana":
                 // Logic for Mana item
                 Debug.Log("Health Used");
                 break;
             default:
                 break;
           /* case ItemType.Health:
              // Debug.Log("Health Used");
                break;
            case ItemType.MANA:
               // Debug.Log("Mana Used");
                break;
            case ItemType.Mete:
               // Debug.Log("Mete used ");
                break;
                
            default:
                break;*/
        }
    }

    public void SetStats(Item item)
    {
        //type = item.type;
        ıtemData = item.ıtemData;
        spriteNeutral = item.spriteNeutral;
        spriteHighlighted = item.spriteHighlighted;

        maxSize = item.maxSize;

        /*switch (type)
        {
            case ItemType.Health:
                GetComponent<Renderer>().material.color = Color.red;
                break;
            case ItemType.MANA:
                GetComponent<Renderer>().material.color = Color.green;
                break;
            case ItemType.Mete:
                GetComponent<Renderer>().material.color = Color.blue;
                break;
        }*/
    }
    /*public Item Combine(Item otherItem)
    {
        if (otherItem.type == combinableWith)
        {
            // Logic to create a new item
            // This could be a simple instantiation or a more complex process
            // depending on your game's logic
            //Item newItem = //... create new item
            //return newItem;
        }
        return null;
    }*/

    public void PickUp()
    {
        Debug.Log("TheObject has been picked");
        Destroy(gameObject);
    }
}
