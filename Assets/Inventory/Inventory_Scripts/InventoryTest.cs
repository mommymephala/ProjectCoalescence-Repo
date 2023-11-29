using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

    }
}
