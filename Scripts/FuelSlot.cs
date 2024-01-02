using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

/// <summary>
/// 화로의 연료 슬롯
/// </summary>
public class FuelSlot : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [SerializeField] Image image;

    public Item _item;
    public Furnace furnace;

    public int itemCount;
    public Text itemCountTxt;
    public GameObject selected;
    public GameObject hilighted;
    public bool clickable;

    public SelectedItem selectedItem;

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
        item = _item;
        SetItemCountText();
    }

    public void SetItemCountText()
    {
        if (itemCount <= 1)
            itemCountTxt.text = "";
        else
            itemCountTxt.text = itemCount.ToString();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (clickable && eventData.button == PointerEventData.InputButton.Left)
        {
            if (item != null && item == selectedItem.item)
                CombineItems();
            else if (selectedItem.item == null ||
                     selectedItem.item != null && selectedItem.item.burningTime > 0)
                SwitchItems();
        }
        else if (clickable && eventData.button == PointerEventData.InputButton.Right)
        {
            if (item != null && selectedItem.item == null)
                SplitItems();
            else if (selectedItem.item != null && (item == null || item == selectedItem.item))
            {
                if (item != null && itemCount >= item.maxStorageCount)
                    return;
                else if (selectedItem.item.burningTime > 0)
                    SplitItem();
            }
        }
    }

    public void SwitchItems()
    {
        Item tempItem = selectedItem.item;
        int tempCount = selectedItem.itemCount;

        selectedItem.item = item;
        selectedItem.itemCount = itemCount;
        selectedItem.SetItemCountText();

        item = tempItem;
        itemCount = tempCount;
        SetItemCountText();
    }

    public void CombineItems()
    {
        itemCount += selectedItem.itemCount;
        if (itemCount > item.maxStorageCount)
        {
            selectedItem.itemCount = itemCount - item.maxStorageCount;
            itemCount = item.maxStorageCount;
        }
        else
        {
            selectedItem.itemCount = 0;
            selectedItem.item = null;
        }
        selectedItem.SetItemCountText();
        SetItemCountText();
    }

    public void SplitItems()
    {
        selectedItem.item = item;
        selectedItem.itemCount = (itemCount / 2) + (itemCount % 2);
        itemCount /= 2;
        if (itemCount == 0)
            item = null;
        selectedItem.SetItemCountText();
        SetItemCountText();
    }

    public void SplitItem()
    {
        item = selectedItem.item;
        selectedItem.itemCount--;
        itemCount++;
        if (selectedItem.itemCount <= 0)
            selectedItem.item = null;
        selectedItem.SetItemCountText();
        SetItemCountText();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(false);
    }
}
