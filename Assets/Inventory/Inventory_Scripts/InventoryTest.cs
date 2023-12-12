using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryTest : MonoBehaviour
{
    private RectTransform _inventoryRect;
    private float _inventoryWidht, _inventoryHeight;
    public int slots;
    public int rows;
    public float slotPaddingLeft, slotPaddingTop;
    public float slotSize;
    public GameObject slotPrefab;
    private static Slot _from, _to;
    private List<GameObject> _allslots;
    // public GameObject iconPrefab;
    private static GameObject _hoverObject;
    public Canvas canvas;
    private float _hoverYOffset;
    public EventSystem eventsystem;
    public GameObject dropItem;
    private static GameObject _playerRef;

    public static int EmptySlots { get; set; }

    private void Awake()
    {
        CreateLayout();
    }

    private void Start()
    {
        canvas.enabled = false;
        _playerRef = GameObject.Find("PlayerWithLoadout");
    }

    private void Update()
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
                Destroy(GameObject.Find("Hover"));
                _to = null;
                _from = null;
               // hoverObject = null;
                
                Debug.Log("çalışıyor");
            }
        }
        
      /*  if (hoverObject != null)
        {
            Vector2 position;
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvas.transform as RectTransform,Input.mousePosition, canvas.worldCamera, out position);

            position.Set(position.x,position.y- hoverYOffset);
            
            hoverObject.transform.position = canvas.transform.TransformPoint((position));
        }*/
    }

    private void CreateLayout()
    {
        
        _allslots = new List<GameObject>();

        _hoverYOffset = slotSize * 0.01f;

        EmptySlots = slots;
        
        _inventoryWidht = (slots / rows) * (slotSize + slotPaddingLeft) + slotPaddingLeft;

        _inventoryHeight = rows * (slotSize + slotPaddingTop) + slotPaddingTop;

        _inventoryRect = GetComponent<RectTransform>();
        
        _inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, _inventoryWidht);
        _inventoryRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, _inventoryHeight);


        int columns = slots / rows;



        for (int y = 0; y < rows; y++)
        {
           // float yPosition = (-2) - slotPaddingTop * (y + 1) - (slotSize * y);

            for (int x = 0; x < columns; x++)
            {
                GameObject newSlot = Instantiate(slotPrefab);

                RectTransform slotRect = newSlot.GetComponent<RectTransform>();

                newSlot.name = "Slot";

                newSlot.transform.SetParent(transform.parent);

                slotRect.localPosition = _inventoryRect.localPosition +
                                         new Vector3(slotPaddingLeft * (x + 1) + (slotSize * x),
                                             -slotPaddingTop * (y + 1) - (slotSize * y));
                
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal,slotSize);
                slotRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical,slotSize);
                
                _allslots.Add(newSlot);
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
            foreach (GameObject slot in _allslots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (!tmp.IsEmpty)
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
            foreach (GameObject slot in _allslots)
            {
                Slot tmp = slot.GetComponent<Slot>();

                if (tmp.IsEmpty)
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
            if (!clicked.GetComponent<Slot>().IsEmpty)
            {
                _from = clicked.GetComponent<Slot>();
                
                _from.GetComponent<Image>().color = Color.gray;

                //hoverObject = (GameObject)Instantiate(iconPrefab);
               // hoverObject.GetComponent<Image>().sprite = clicked.GetComponent<Image>().sprite;
                //hoverObject.name = "Hover";

               // RectTransform hoverTransfrom = hoverObject.GetComponent<RectTransform>();
                //RectTransform clickedTransform = clicked.GetComponent<RectTransform>();
                
                
                //hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, clickedTransform.sizeDelta.x);
                //hoverTransfrom.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, clickedTransform.sizeDelta.y);
                
                
                //hoverObject.transform.SetParent(GameObject.Find("Canvas").transform, true);
               // hoverObject.transform.localScale = from.gameObject.transform.localScale;

            }
        }
        else if (_to == null)
        {
            _to = clicked.GetComponent<Slot>();
            Destroy(GameObject.Find("Hover"));
        }

        if (_to != null && _from != null)
        {
            Stack<Item> TmpTo = new Stack<Item>(_to.Items);
            _to.AddItems(_from.Items);

            if (TmpTo.Count == 0)
            {
                _from.ClearSlot();
            }
            else
            {
                _from.AddItems(TmpTo);
            }

            _from.GetComponent<Image>().color = Color.white;
            _to = null;
            _from = null;
            //hoverObject = null;
        }
    }
    
}
