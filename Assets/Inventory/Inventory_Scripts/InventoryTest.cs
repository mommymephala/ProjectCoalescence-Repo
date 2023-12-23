using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;
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

    [FormerlySerializedAs("SlotPrefab")] public GameObject slotPrefab;

    private static Slot _from, _to;

    private List<GameObject> allslots;
    

    public Canvas canvas;
    

    public EventSystem eventsystem;

    public GameObject dropItem;
    private static GameObject _playerRef;
    
    private static int _emptySlots;
    private Image ımage;

    public static int EmptySlots
    { 
        get { return _emptySlots; }
        set { _emptySlots = value; }
    }

    private void Awake()
    {
        CreateLayout();
    }

    // Start is called before the first frame update
    void Start()
    {
        canvas.enabled = false;
        _playerRef = GameObject.Find("PlayerWithLoadout");
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            if (!eventsystem.IsPointerOverGameObject(-1) && _from != null)
            {
                 _from.GetComponent<Image>().color = Color.white;

                foreach (Item item in _from.Items)
                {
                    float angle = UnityEngine.Random.Range(0.0f, Mathf.PI * 2);

                    Vector3 v = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle));

                    v *= 4;

                   GameObject tmpDrop = GameObject.Instantiate(dropItem, _playerRef.transform.position - v, quaternion.identity);

                   tmpDrop.GetComponent<Item>().SetStats(item);
                }
                _from.ClearSlot();
               
                _to = null;
                _from = null;
                
                Debug.Log("çalışıyor");
            }
        }
        
    }

    private void CreateLayout()
    {
        allslots = new List<GameObject>();
        EmptySlots = slots;
        
        inventoryWidht = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;
        inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;
        
        inventoryRect = GetComponent<RectTransform>();
        
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, inventoryWidht);
        inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, inventoryHeight);
        
        int columns = slots / rows;
        
        for (int y = 0; y < rows; y++)
        {
            for (int x = 0; x < columns; x++)
            {
                GameObject newSlot = Instantiate(slotPrefab);

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
                    if (tmp.CurrentItem.ıtemData == item.ıtemData && tmp.IsAvailable)
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
        if (_from == null)
        {
            if (!clicked.GetComponent<Slot>().isEmpty)
            {
                _from = clicked.GetComponent<Slot>();
            }
        }
        else if (_to == null)
        {
            _to = clicked.GetComponent<Slot>();
        }

        if (_to != null && _from != null)
        {
            Stack<Item> tmpTo = new Stack<Item>(_to.Items);
            _to.AddItems(_from.Items);

            if (tmpTo.Count == 0)
            {
                _from.ClearSlot();
            }
            else
            {
                _from.AddItems(tmpTo);
            }

            _from.GetComponent<Image>().color = Color.white;
            _to = null;
            _from = null;
          
        }
    }
    
}
