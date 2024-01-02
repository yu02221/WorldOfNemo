using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Slot : MonoBehaviour
    , IPointerClickHandler
    , IPointerEnterHandler
    , IPointerExitHandler
{
    [SerializeField] Image image;

    public Item _item;

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
            // item에 맞는 이미지 표시
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

    private void Update()
    {
        if (clickable && hilighted.activeSelf && Input.GetKeyDown(KeyCode.Q))
        {
            item = null;
            itemCount = 0;
            SetItemCountText();
        }
    }
    /// <summary>
    /// 아이템 수량에 알맞게 텍스트 수정
    /// </summary>
    public void SetItemCountText()
    {
        if (itemCountTxt == null)
            return;
        if (itemCount <= 1)
            itemCountTxt.text = "";
        else
            itemCountTxt.text = itemCount.ToString();
    }
    // 슬롯 클릭 시
    public void OnPointerClick(PointerEventData eventData)
    {
        // 클릭 가능한 슬롯 좌클릭시
        if (clickable && eventData.button == PointerEventData.InputButton.Left)
        {
            // 들고 있는 아이템과 슬롯의 아이템이 같은 경우 합치고 다른 경우 교환
            if (item != null && item == selectedItem.item)
                CombineItems();
            else
                SwitchItems();
        }
        // 클릭 가능한 슬롯 우클릭시
        else if (clickable && eventData.button == PointerEventData.InputButton.Right)
        {
            // 들고 있는 아이템이 없을 경우 슬롯의 아이템을 반 나누어 가져옴
            if (item != null && selectedItem.item == null)
                SplitItems();
            // 빈 슬롯이거나 들고 있는 아이템과 슬롯 아이템이 같을 경우 들고있는 아이템 하나를 슬롯에 옮김 
            else if (selectedItem.item != null && (item == null || item == selectedItem.item))
            {
                if (item != null && itemCount >= item.maxStorageCount)
                    return;
                else
                    SplitItem();
            }
        }
    }
    /// <summary>
    /// 들고 있는 아이템과 슬롯의 아이템을 바꿈
    /// </summary>
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
    /// <summary>
    /// 들고있는 아이템을 슬롯아이템에 합침
    /// </summary>
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
    /// <summary>
    /// 슬롯 아이템을 반 나누어서 가져옴
    /// </summary>
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
    /// <summary>
    /// 들고있는 아이템 중 한개를 슬롯에 놓음
    /// </summary>
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
    // 슬롯 위에 커서가 올라갔을 때 하이라이트 활성화
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(true);
    }
    // 커서가 슬롯을 빠져 나갔을 때 하이라이트 비활성화
    public void OnPointerExit(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(false);
    }
}
