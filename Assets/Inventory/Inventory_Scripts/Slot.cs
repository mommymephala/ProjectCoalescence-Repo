using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{

    private Stack<Item> items;

    public Stack<Item> Items
    {
        get { return items; }

        set { items = value; }
    }

    public Text stackTxt;

    public Sprite slotEmpty;

    public Sprite slotHighlight;

    public bool isEmpty
    {
        get { return items.Count == 0; }
    }

    public bool IsAvailable
    {
        get
        {
            return CurrentItem.maxSize > items.Count;
        }
    }
    public Item CurrentItem
    {
        get { return items.Peek();}
    }
    // Start is called before the first frame update
    void Start()
    {
        items = new Stack<Item>();

        RectTransform slotRect = GetComponent<RectTransform>();

        RectTransform txtRect = stackTxt.GetComponent<RectTransform>();

        int txtScleFactor = (int)(slotRect.sizeDelta.x * 0.60);

        stackTxt.resizeTextMaxSize = txtScleFactor;
        stackTxt.resizeTextMinSize = txtScleFactor;
        
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);


    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddItem(Item item)
    {
        items.Push(item);
        if (items.Count > 1)
        {
            stackTxt.text = items.Count.ToString();
        }
        
        ChangeSprite(item.spriteNeutral, item.spriteHighlighted);
    }
    public void AddItems(Stack<Item> items)
    {
        this.items = new Stack<Item>(items);
        
        stackTxt.text = items.Count > 1 ? items.Count.ToString() : String.Empty;
        
        ChangeSprite(CurrentItem.spriteNeutral, CurrentItem.spriteHighlighted);


    }
    
    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;

        SpriteState st = new SpriteState();

        st.highlightedSprite = highlight;
        st.pressedSprite = neutral;

        GetComponent<Button>().spriteState = st;
    }

  

    private void UseItem()
    {
        if (!isEmpty)
        {
            items.Pop().Use();

            stackTxt.text = items.Count > 1 ? items.Count.ToString() : String.Empty;

            if (isEmpty)
            {
                ChangeSprite(slotEmpty, slotHighlight);

                InventoryTest.EmptySlots++;
            }
        }
    }

    public void ClearSlot()
    {
        items.Clear();
        ChangeSprite(slotEmpty,slotHighlight);
        stackTxt.text = string.Empty;

    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            UseItem();
        }
        
    }
}
