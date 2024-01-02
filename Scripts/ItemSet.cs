using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemSet : MonoBehaviour
{
    public List<Item> items = new List<Item>();

    public Dictionary<string, Item> iSet;

    private void Start()
    {
        iSet = new Dictionary<string, Item>();
        foreach (var item in items)
            iSet.Add(item.itemName, item);
    }
}
