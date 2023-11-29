using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{
    MANA,
    Health
};
public class Item : MonoBehaviour
{

    public ItemType type;

    public Sprite spriteNeutral;

    public Sprite spriteHighlighted;

    public int maxSize;
    // Start is called before the first frame update
    public void Use()

    {
        switch (type)
        {
            case ItemType.Health:
                Debug.Log("Health Used");
                break;
            case ItemType.MANA:
                Debug.Log("Mana Used");

                break;
            default:
                break;
        }
    }
}
