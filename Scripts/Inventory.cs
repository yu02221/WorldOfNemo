using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Slot[] slots;    // 인벤토리에 있는 슬롯 목록
    public Inventory nextInventory;     // 인벤토리가 가득 찼을 때 아이템을 넘겨 줄 인벤토리

    // 하이어라키 창에 올렸을 때 자동으로 실행
    private void OnValidate()
    {
        // 자식 오브젝트 중 슬롯들을 슬롯 목록에 등록
        slots = transform.GetComponentsInChildren<Slot>();
    }
    /// <summary>
    /// 인벤토리에 아이템 추가
    /// </summary>
    /// <param name="_item">추가할 아이템</param>
    /// <param name="count">추가할 아이템 수량</param>
    public void AddItem(Item _item, int count)
    {
        int addIdx = -1;    // 슬롯 목록 중 저장할 위치 인덱스
        // 저장할 위치 검색
        for (int i = 0; i < slots.Length; i++)
        {   // 이미 기존에 있는 아이템인 경우
            if (slots[i].item == _item)
            {   
                if (slots[i].itemCount + count <= _item.maxStorageCount)
                {   // 아이템이 최대 수량보다 적으면 이곳에 저장
                    addIdx = i;
                    break;
                }
                else
                {   // 최대 수량을 넘으면 최대 수량만큼 이곳에 자장하고 나머지를 저장할 곳을 다시 검색
                    count -= _item.maxStorageCount - slots[i].itemCount;
                    slots[i].itemCount = _item.maxStorageCount;
                }
            }
        }
        // 기존에 있는 아이템이 아닌 경우
        if (addIdx == -1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {   // 빈 슬롯이 있으면 이곳에 저장
                    addIdx = i;
                    break;
                }
            }
            if (addIdx == -1)
            {   // 빈 슬롯이 없으면 다음 인벤토리에 넘겨줌
                if (nextInventory != null)
                    nextInventory.AddItem(_item, count);
                else
                    print("모든 인벤토리가 가득 찼습니다.");
                return;
            }
        }
        if (slots[addIdx].item == null)
            slots[addIdx].item = _item;
        slots[addIdx].itemCount += count;
        slots[addIdx].SetItemCountText();
    }

    public void ResetInventory()
    {
        foreach (var slot in slots)
        {
            slot.itemCount = 0;
            slot.item = null;
            slot.SetItemCountText();
        }
        if (nextInventory != null)
            nextInventory.ResetInventory();
    }
}
