using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SelectedItem : MonoBehaviour
{
    [SerializeField] Image image;

    public Item _item;

    public int itemCount;
    public Text itemCountTxt;

    public Item item
    {
        get { return _item; }
        set
        {
            _item = value;
            if (_item != null)
            {
                image.sprite = item.itemImage;
                image.color = new Color(1, 1, 1, 1);
            }
            else
            {
                image.color = new Color(1, 1, 1, 0);
            }
        }
    }


    private void Start()
    {
        item = null;
    }

    private void Update()
    {
        transform.position = Input.mousePosition;
    }

    public void SetItemCountText()
    {
        if (itemCount <= 1)
            itemCountTxt.text = "";
        else
            itemCountTxt.text = itemCount.ToString();
    }
}
