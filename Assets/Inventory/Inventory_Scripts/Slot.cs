using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

public class Slot : MonoBehaviour
{

    private Stack<Item> items;

    public Text stackTxt;

    public Sprite slotEmpty;

    public Sprite slotHighlight;

    public bool isEmpty
    {
        get { return items.Count == 0; }
    }
    // Start is called before the first frame update
    void Start()
    {
        items = new Stack<Item>();

        RectTransform slotRect = GetComponent<RectTransform>();

        RectTransform txtRect = GetComponent<RectTransform>();

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
    private void ChangeSprite(Sprite neutral, Sprite highlight)
    {
        GetComponent<Image>().sprite = neutral;

        SpriteState st = new SpriteState();

        st.highlightedSprite = highlight;
        st.pressedSprite = neutral;

        GetComponent<Button>().spriteState = st;
    }   
}
