using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Furnace : CraftingInventory
{
    public Slider arrowSlider;
    public Slider fireSlider;
    public FuelSlot fuelSlot;
    public Slot itemSlot;
    public ItemSet itemSet;

    private FurnaceState fState = FurnaceState.Idle;
    private float maxBurnTime;
    private float burnTime;
    private float bakeTime;
    private bool fuelIn = false;
    private bool itemIn = false;
    private Item curItem;

    private void Start()
    {
        recipes = CSVReader.Read("Furnace");
    }

    private void Update()
    {
        if (curItem != itemSlot.item)
            ResetItem();

        switch (fState)
        {
            case FurnaceState.Idle :
                Idle();
                break;
            case FurnaceState.Burn:
                BurnItem();
                break;
            case FurnaceState.CoolDown:
                CoolDown();
                break;
        }
    }

    private void Idle()
    {
        if (!fuelIn && fuelSlot.item != null)
            fuelIn = true;

        if (fuelSlot.item == null)
            fuelIn = false;

        if (!itemIn && CheckRecipes(itemSlot.item))
            itemIn = true;

        if (fuelIn && itemIn)
        {
            burnTime = fuelSlot.item.burningTime;
            maxBurnTime = burnTime;
            if (--fuelSlot.itemCount == 0)
                fuelSlot.item = null;
            fuelSlot.SetItemCountText();

            bakeTime = 0;
            fState = FurnaceState.Burn;
        }
    }

    private void BurnItem()
    {
        if (itemSlot.item == null)
        {
            bakeTime = 0;
            arrowSlider.value = 0;
            itemIn = false;
            fState = FurnaceState.CoolDown;
            return;
        }

        if (burnTime > 0)
        {
            burnTime -= Time.deltaTime;
            bakeTime += Time.deltaTime;
            fireSlider.value = burnTime / maxBurnTime;
            arrowSlider.value = bakeTime / 10f;

            if (bakeTime >= 10f)
            {
                bakeTime = 0;
                GetResultItem();
            }
        }
        else
        {
            if (fuelSlot.item != null)
            {
                burnTime = fuelSlot.item.burningTime;
                maxBurnTime = burnTime;

                if (--fuelSlot.itemCount == 0)
                    fuelSlot.item = null;
                fuelSlot.SetItemCountText();
            }
            else
            {
                fuelIn = false;
                fState = FurnaceState.Idle;
            }
        }
    }

    private void CoolDown()
    {
        if (burnTime > 0)
        {
            if (CheckRecipes(itemSlot.item))
            {
                itemIn = true;
                bakeTime = 0;
                fState = FurnaceState.Burn;
            }
            else
            {
                burnTime -= 3 * Time.deltaTime;
                fireSlider.value = burnTime / maxBurnTime;
            }
        }
        else
        {
            burnTime = 0;
            fState = FurnaceState.Idle;
        }
    }

    private bool CheckRecipes(Item item)
    {
        if (item == null)
            return false;

        for (int i = 0; i < recipes.Count; i++)
        {
            if (item.name == recipes[i]["Item"].ToString()
                && (resultSlot.item == null || resultSlot.item == item))
            {
                resultItem = itemSet.iSet[recipes[i]["ResultItem"].ToString()];
                curItem = item;
                return true;
            }
        }
        return false;
    }

    private  new void GetResultItem()
    {
        if (--itemSlot.itemCount == 0)
            itemSlot.item = null;
        itemSlot.SetItemCountText();

        resultSlot.item = resultItem;
        resultSlot.itemCount++;
        resultSlot.SetItemCountText();
    }

    public void ResetItem()
    {
        curItem = itemSlot.item;
        bakeTime = 0;
        arrowSlider.value = 0;
        fState = FurnaceState.CoolDown;
    }

    enum FurnaceState { Idle, Burn, CoolDown }
}