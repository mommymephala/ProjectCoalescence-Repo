using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
//using UnityEngine.UIElements;
using UnityEngine.UI;



public class InventoryTest : MonoBehaviour
{
    private RectTransform inventoryRect;
    

    private float inventoryWidht, inventoryHeight;

    public int slots;

    public int rows;

    public float slotPaddingLeft, slotPaddingTop;

    public float slotSize;

    public GameObject SlotPrefab;

    private static Slot from, to;

    private List<GameObject> allslots;

    public GameObject iconPrefab;

    private static GameObject hoverObject;

    public Canvas canvas;

    private float hoverYOffset;

    public EventSystem eventsystem;

    public GameObject dropItem;
    private static GameObject playerRef;
    
    private static int emptySlots;

    public static int EmptySlots
    {
        get { return emptySlots; }
        set { emptySlots = value; }

    }

    private void Awake()
    {
        CreateLayout();
    }

    // Start is called before the first frame update
    void Start()
    {

        canvas.enabled = false;
        playerRef = GameObject.Find("PlayerWithLoadout");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!eventsystem.IsPointerOverGameObject(-1) && from != null)
            {
                from.GetComponent<Image>().color = Color.white;

                foreach (Item item in from.Items)
                {
                    float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2);

                    Vector3 v = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                    v *= 4;

                   GameObject tmpDrop = GameObject.Instantiate(dropItem, playerRef.transform.position - v, quaternion.identity);

                   tmpDrop.GetComponent<Item>().SetStats(item);
                }
                from.ClearSlot();
                Destroy(GameObject.Find("Hover"));
                to = null;
                from = null;
                hoverObject = null;
                
                Debug.Log("çalışıyor");
            }
        }
        
        if (hoverObject != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,Input.mousePosition, canvas.worldCamera, out position);

            position.Set(position.x,position.y- hoverYOffset);
            
            hoverObject.transform.position = canvas.transform.TransformPoint((position));
        }
    }

    private void CreateLayout()
    {
        
        allslots = new List<GameObject>();

        hoverYOffset = slotSize * 0.01f;

        EmptySlots = slots;
        
        inventoryWidht = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;

        inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;

        inventoryRect = GetComponent<RectTransform>();
        
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidht);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);


        int columns = slots / rows;



        for (int y = 0; y < rows; y++)
        {
           // float yPosition = (-2) - slotPaddingTop * (y + 1) - (slotSize * y);

            for (int x = 0; x < columns; x++)
            {
                GameObject newSlot = Instantiate(SlotPrefab);

                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";

                newSlot.transform.SetParent(transform.parent);

                slotRect.localPosition = inventoryRect.localPosition +
                                         new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x),
                                             -slotPaddingTop * (y + 1) - (slotSize * y));
                
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,slotSize);
                
                allslots.Add(newSlot);
            }

        }

    }

    public bool AddItem(Item item)
    {
        if (item.maxSize == 1)
        {
            PlaceEmpty((item));
            return true;
        }
        else
        {
            foreach (GameObject slot in allslots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (!tmp.isEmpty)
                {
                    if (tmp.CurrentItem.type == item.type && tmp.IsAvailable)
                    {
                         tmp.AddItem(item);

                         return true;
                    } 
                }
            }

            if (EmptySlots > 0)
            {
                PlaceEmpty(item);
            }
        }

        return false;
    }
    
    private bool PlaceEmpty(Item item)
    {
        if (EmptySlots > 0)
        {
            foreach (GameObject slot in allslots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (tmp.isEmpty)
                {
                    tmp.AddItem(item);
                    EmptySlots--;

                    return true;
                }
            }
            
        }

        return false;
    }

    public void MoveItem(GameObject clicked)
    {
        if (from == null)
        {
            if (!clicked.GetComponent<Slot>().isEmpty)
            {
                from = clicked.GetComponent<Slot>();
                
                from.GetComponent<Image>().color = Color.gray;

                hoverObject = (GameObject)Instantiate(iconPrefab);
                hoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
                hoverObject.name = "Hover";

                RectTransform hoverTransfrom = hoverObject.GetComponent<RectTransform>();
                RectTransform clickedTransform = clicked.GetComponent<RectTransform>();
                
                
                hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
                hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);
                
                
                hoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
                hoverObject.transform.localScale = from.gameObject.transform.localScale;

            }
        }
        else if (to == null)
        {
            to = clicked.GetComponent<Slot>();
            Destroy(GameObject.Find("Hover"));
        }

        if (to != null && from != null)
        {
            Stack<Item> TmpTo = new Stack<Item>(to.Items);
            to.AddItems(from.Items);

            if (TmpTo.Count == 0)
            {
                from.ClearSlot();
            }
            else
            {
                from.AddItems(TmpTo);
            }

            from.GetComponent<Image>().color = Color.white;
            to = null;
            from = null;
            hoverObject = null;
        }
    }
    
}
