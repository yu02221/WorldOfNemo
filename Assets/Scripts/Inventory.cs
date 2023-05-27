using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    public Slot[] slots;    // �κ��丮�� �ִ� ���� ���
    public Inventory nextInventory;     // �κ��丮�� ���� á�� �� �������� �Ѱ� �� �κ��丮

    // ���̾��Ű â�� �÷��� �� �ڵ����� ����
    private void OnValidate()
    {
        // �ڽ� ������Ʈ �� ���Ե��� ���� ��Ͽ� ���
        slots = transform.GetComponentsInChildren<Slot>();
    }
    /// <summary>
    /// �κ��丮�� ������ �߰�
    /// </summary>
    /// <param name="_item">�߰��� ������</param>
    /// <param name="count">�߰��� ������ ����</param>
    public void AddItem(Item _item, int count)
    {
        int addIdx = -1;    // ���� ��� �� ������ ��ġ �ε���
        // ������ ��ġ �˻�
        for (int i = 0; i < slots.Length; i++)
        {   // �̹� ������ �ִ� �������� ���
            if (slots[i].item == _item)
            {   
                if (slots[i].itemCount + count <= _item.maxStorageCount)
                {   // �������� �ִ� �������� ������ �̰��� ����
                    addIdx = i;
                    break;
                }
                else
                {   // �ִ� ������ ������ �ִ� ������ŭ �̰��� �����ϰ� �������� ������ ���� �ٽ� �˻�
                    count -= _item.maxStorageCount - slots[i].itemCount;
                    slots[i].itemCount = _item.maxStorageCount;
                }
            }
        }
        // ������ �ִ� �������� �ƴ� ���
        if (addIdx == -1)
        {
            for (int i = 0; i < slots.Length; i++)
            {
                if (slots[i].item == null)
                {   // �� ������ ������ �̰��� ����
                    addIdx = i;
                    break;
                }
            }
            if (addIdx == -1)
            {   // �� ������ ������ ���� �κ��丮�� �Ѱ���
                if (nextInventory != null)
                    nextInventory.AddItem(_item, count);
                else
                    print("��� �κ��丮�� ���� á���ϴ�.");
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
