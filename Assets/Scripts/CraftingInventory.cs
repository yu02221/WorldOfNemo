using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 제작 재료를 올려놓는 인벤토리
/// </summary>
public class CraftingInventory : MonoBehaviour
{
    public Slot[] slots;     // 하위 슬롯 목록
    protected string[] items;   // 올려놓은 재료 아이템 이름 목록
    public ItemSet itemset;     // 결과 아이템을 이름으로 참조하기 위한 아이템셋
    
    protected List<Dictionary<string, object>> recipes;     // csv 파일에서 읽어 올 레시피 목록
    protected Item resultItem;      // 제작된 아이템
    protected int resultItemCount;  // 제작된 아이템 수량
    public ResultSlot resultSlot;   // 제작된 아이템을 나타낼 슬롯


    private void OnValidate()
    {   // 스크립트 등록 시 실행, 자식 오브젝트중 슬롯을 slots에 등록
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
        // items에 아이템 이름 초기화
        for (int i = 0; i < slots.Length; i++)
        {
            if (slots[i].item != null)
                items[i] = slots[i].item.itemName;
            else
                items[i] = "null";
        }
        // 레시피 검사
        GetResultItem();

        // resultItem을 resultSlot에 나타냄
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
    /// 레시피와 비교하여 일치하면 resultItem 및 resultItemCount 초기화
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
