using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class InventoryTest : MonoBehaviour
{
    private RectTransform inventoryRect;
    

    private float inventoryWidht, inventoryHeight;

    public int slots;

    public int rows;

    public float slotPaddingLeft, slotPaddingTop;

    public float slotSize;

    public GameObject SlotPrefab;

    private List<GameObject> allslots;
    
    // Start is called before the first frame update
    void Start()
    {
        CreateLayout();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void CreateLayout()
    {
        
        allslots = new List<GameObject>();

        inventoryWidht = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;

        inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;

        inventoryRect = GetComponent<RectTransform>();
        
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidht);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);


        int columns = slots / rows;



        for (int y = 0; y < rows; y++)
        {
            float yPosition = (-2) - slotPaddingTop * (y + 1) - (slotSize * y);

            for (int x = 0; x < columns; x++)
            {
                GameObject newSlot = (GameObject)Instantiate(SlotPrefab);

                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";

                newSlot.transform.SetParent(this.transform.parent);

                slotRect.localPosition = inventoryRect.localPosition +
                                         new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x),
                                             -slotPaddingTop * (y + 1) - (slotSize * y));
                
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,slotSize);
                
                allslots.Add(newSlot);
            }

        }

    }
}
