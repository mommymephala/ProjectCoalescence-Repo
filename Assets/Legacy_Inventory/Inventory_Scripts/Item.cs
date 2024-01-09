using Interfaces;
using UnityEngine;

public enum ItemType
{
    Mana,
    Health,
    Mete
};
public class Item : MonoBehaviour, IPickUp
{
    
    public ItemType type;
    public Sprite spriteNeutral;
    public Sprite spriteHighlighted;
    public int maxSize;
    
    public void Use()

    {
        switch (type)
        {
            case ItemType.Health:
              // Debug.Log("Health Used");
                break;
            case ItemType.Mana:
               // Debug.Log("Mana Used");
                break;
            case ItemType.Mete:
               // Debug.Log("Mete used ");
                break;
            default:
                break;
        }
    }

    public void SetStats(Item item)
    {
        type = item.type;

        spriteNeutral = item.spriteNeutral;
        spriteHighlighted = item.spriteHighlighted;

        maxSize = item.maxSize;

        /*switch (type)
        {
            case ItemType.Health:
                GetComponent<Renderer>().material.color = Color.red;
                break;
            case ItemType.Mana:
                GetComponent<Renderer>().material.color = Color.green;
                break;
            case ItemType.Mete:
                GetComponent<Renderer>().material.color = Color.blue;
                break;
        }*/
    }


    public void PickUp()
    {
        Debug.Log("TheObject has been picked");
        Destroy(gameObject);
    }
}
