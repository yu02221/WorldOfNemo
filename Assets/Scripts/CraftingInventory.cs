using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// ���� ��Ḧ �÷����� �κ��丮
/// </summary>
public class CraftingInventory : MonoBehaviour
{
    public Slot[] slots;     // ���� ���� ���
    protected string[] items;   // �÷����� ��� ������ �̸� ���
    public ItemSet itemset;     // ��� �������� �̸����� �����ϱ� ���� �����ۼ�
    
    protected List<Dictionary<string, object>> recipes;     // csv ���Ͽ��� �о� �� ������ ���
    protected Item resultItem;      // ���۵� ������
    protected int resultItemCount;  // ���۵� ������ ����
    public ResultSlot resultSlot;   // ���۵� �������� ��Ÿ�� ����


    private void OnValidate()
    {   // ��ũ��Ʈ ��� �� ����, �ڽ� ������Ʈ�� ������ slots�� ���
        slots = transform.GetComponentsInChildren<Slot>();
    }

    private void Start()
    {
        items = new string[slots.Length];
        if (slots.Length == 4)
            recipes = CSVReader.Read("CraftingInven");
        else if (slots.Length == 9)
            recipes = CSVReader.Read("CraftingTable");
    }

    private void Update()
    {
        // items�� ������ �̸� �ʱ�ȭ
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
                items[i] = slots[i].item.itemName;
            else
                items[i] = "null";
        }
        // ������ �˻�
        GetResultItem();

        // resultItem�� resultSlot�� ��Ÿ��
        if (resultItem != null)
        {
            resultSlot.item = resultItem;
            resultSlot.itemCount = resultItemCount;
        }
        else
        {
            resultSlot.item = null;
            resultSlot.itemCount = 0;
        }
        resultSlot.SetItemCountText();
    }
    /// <summary>
    /// �����ǿ� ���Ͽ� ��ġ�ϸ� resultItem �� resultItemCount �ʱ�ȭ
    /// </summary>
    protected void GetResultItem()
    {
        int i = 0;
        for (; i < recipes.Count; i++)
        {
            bool inRecipe = true;
            for (int j = 0; j < items.Length; j++)
            {
                string itemIdx = "Item" + j;
                if (items[j] != recipes[i][itemIdx].ToString())
                {
                    inRecipe = false;
                    break;
                }
            }
            if (inRecipe)
            {
                resultItem = itemset.iSet[recipes[i]["ResultItem"].ToString()];
                resultItemCount = int.Parse(recipes[i]["Count"].ToString());
                break;
            }
        }
        if (i == recipes.Count)
            resultItem = null;
    }
}
