using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Slot : MonoBehaviour, IPointerClickHandler
{
    public Stack<Item> Items { get; private set; }
    public Text stackTxt;
    public Sprite slotEmpty;
    public Sprite slotHighlight;

    public bool IsEmpty => Items.Count == 0;

    public bool IsAvailable => CurrentItem.maxSize > Items.Count;

    public Item CurrentItem => Items.Peek();

    // Start is called before the first frame update
    private void Start()
    {
        Items = new Stack<Item>();

        RectTransform slotRect = GetComponent<RectTransform>();

        RectTransform txtRect = stackTxt.GetComponent<RectTransform>();

        int txtScleFactor = (int)(slotRect.sizeDelta.x * 0.60);

        stackTxt.resizeTextMaxSize = txtScleFactor;
        stackTxt.resizeTextMinSize = txtScleFactor;
        
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, slotRect.sizeDelta.y);
        txtRect.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, slotRect.sizeDelta.x);


    }

    public void AddItem(Item item)
    {
        Items.Push(item);
        if (Items.Count > 1)
        {
            stackTxt.text = Items.Count.ToString();
        }
        ChangeSprite(item.spriteNeutral, item.spriteHighlighted);
    }
    public void AddItems(Stack<Item> items)
    {
        this.Items = new Stack<Item>(items);
        
        stackTxt.text = items.Count > 1 ? items.Count.ToString() : String.Empty;
        
        ChangeSprite(CurrentItem.spriteNeutral, CurrentItem.spriteHighlighted);
        
    }
    
    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;

        var st = new SpriteState
        {
            highlightedSprite = highlight,
            pressedSprite = neutral
        };

        GetComponent<Button>().spriteState = st;
    }
    
    private void UseItem()
    {
        if (!IsEmpty)
        {
            Items.Pop().Use();

            stackTxt.text = Items.Count > 1 ? Items.Count.ToString() : String.Empty;

            if (IsEmpty)
            {
                ChangeSprite(slotEmpty, slotHighlight);

                InventoryTest.EmptySlots++;
            }
        }
    }

    public void ClearSlot()
    {
        Items.Clear();
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
