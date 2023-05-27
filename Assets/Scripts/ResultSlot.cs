using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ResultSlot : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    public CraftingInventory craftingInven;

    [SerializeField] Image image;
    public Item _item;
    public int itemCount;
    public Text itemCountTxt;
    public GameObject hilighted;

    public SelectedItem selectedItem;

    public PlayerStatus ps;

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

    private void BringItems()
    {
        selectedItem.item = item;
        selectedItem.itemCount += itemCount;
        selectedItem.SetItemCountText();
        item = null;
        itemCount = 0;
        SetItemCountText();
        if (craftingInven.name == "FurnaceInven")
        {
            ps.GetExp(selectedItem.itemCount);
            return;
        }
        for (int i = 0; i < craftingInven.slots.Length; i++)
        {
            if (craftingInven.slots[i].item != null)
            {
                craftingInven.slots[i].itemCount--;
                if (craftingInven.slots[i].itemCount == 0)
                    craftingInven.slots[i].item = null;
                craftingInven.slots[i].SetItemCountText();
            }
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (item == null)
            return;
        if (selectedItem.item == item && 
            selectedItem.itemCount + itemCount <= selectedItem.item.maxStorageCount 
            || selectedItem.item == null)
            BringItems();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        hilighted.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        hilighted.SetActive(false);
    }

    public void SetItemCountText()
    {
        if (itemCount <= 1)
            itemCountTxt.text = "";
        else
            itemCountTxt.text = itemCount.ToString();
    }
}
