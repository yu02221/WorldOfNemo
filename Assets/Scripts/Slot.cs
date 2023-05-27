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
            // item�� �´� �̹��� ǥ��
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
    /// ������ ������ �˸°� �ؽ�Ʈ ����
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
    // ���� Ŭ�� ��
    public void OnPointerClick(PointerEventData eventData)
    {
        // Ŭ�� ������ ���� ��Ŭ����
        if (clickable && eventData.button == PointerEventData.InputButton.Left)
        {
            // ��� �ִ� �����۰� ������ �������� ���� ��� ��ġ�� �ٸ� ��� ��ȯ
            if (item != null && item == selectedItem.item)
                CombineItems();
            else
                SwitchItems();
        }
        // Ŭ�� ������ ���� ��Ŭ����
        else if (clickable && eventData.button == PointerEventData.InputButton.Right)
        {
            // ��� �ִ� �������� ���� ��� ������ �������� �� ������ ������
            if (item != null && selectedItem.item == null)
                SplitItems();
            // �� �����̰ų� ��� �ִ� �����۰� ���� �������� ���� ��� ����ִ� ������ �ϳ��� ���Կ� �ű� 
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
    /// ��� �ִ� �����۰� ������ �������� �ٲ�
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
    /// ����ִ� �������� ���Ծ����ۿ� ��ħ
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
    /// ���� �������� �� ����� ������
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
    /// ����ִ� ������ �� �Ѱ��� ���Կ� ����
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
    // ���� ���� Ŀ���� �ö��� �� ���̶���Ʈ Ȱ��ȭ
    public void OnPointerEnter(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(true);
    }
    // Ŀ���� ������ ���� ������ �� ���̶���Ʈ ��Ȱ��ȭ
    public void OnPointerExit(PointerEventData eventData)
    {
        if (clickable)
            hilighted.SetActive(false);
    }
}
