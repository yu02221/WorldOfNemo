using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuManager : MonoBehaviour
{
    public GameObject gameMenuWindow;
    public Inventory hInven;
    public SelectedItem sItem;
    Inventory[] invens;
    CraftingInventory cInven;
    CraftingInventory craftingTable;

    public GameObject inventoryWindow;
    public GameObject craftingTableWindow;
    public GameObject furnaceWindow;

    private PlayerMove pm;

    private void Start()
    {
        invens = inventoryWindow.GetComponentsInChildren<Inventory>();
        cInven = inventoryWindow.GetComponentInChildren<CraftingInventory>();
        craftingTable = craftingTableWindow.GetComponentInChildren<CraftingInventory>();
        pm = GameObject.Find("Player").GetComponent<PlayerMove>();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (craftingTableWindow.activeSelf)
                CloseCraftingTable();
            else if (furnaceWindow.activeSelf)
                CloseFurnace();
            else if (inventoryWindow.activeSelf)
                CloseInventory();
            else if(gameMenuWindow.activeSelf)
                CloseGameMenu();
            else
                OpenGameMenu();
        }

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (craftingTableWindow.activeSelf)
                CloseCraftingTable();
            else if (furnaceWindow.activeSelf)
                CloseFurnace();
            else if (inventoryWindow.activeSelf)
                CloseInventory();
            else if(!gameMenuWindow.activeSelf)
                OpenInventory();
        }
    }
    public void QuitToTitle()
    {
        Time.timeScale = 1f;
        SceneManager.LoadScene("Menu");
    }

    public void CloseGameMenu()
    {
        gameMenuWindow.SetActive(false);
        Time.timeScale = 1f;
        Cursor.visible = false;
    }

    public void OpenGameMenu()
    {
        gameMenuWindow.SetActive(true);
        Time.timeScale = 0f;
        Cursor.visible = true;
    }

    public void CloseInventory()
    {
        foreach (var inven in invens)
            foreach (var slot in inven.slots)
                slot.hilighted.SetActive(false);
        foreach (var slot in cInven.slots)
        {
            if (slot.item != null)
            {
                hInven.AddItem(slot.item, slot.itemCount);
                slot.item = null;
                slot.itemCount = 0;
                slot.SetItemCountText();
            }
            slot.hilighted.SetActive(false);
        }
        if (sItem.item != null)
        {
            hInven.AddItem(sItem.item, sItem.itemCount);
            sItem.item = null;
            sItem.itemCount = 0;
            sItem.SetItemCountText();
        }
        
        inventoryWindow.SetActive(false);

        pm.playerState = PlayerState.Idle;
        Cursor.visible = false;
    }
    public void CloseCraftingTable()
    {
        foreach (var slot in craftingTable.slots)
        {
            if (slot.item != null)
            {
                hInven.AddItem(slot.item, slot.itemCount);
                slot.item = null;
                slot.itemCount = 0;
                slot.SetItemCountText();
            }
            slot.hilighted.SetActive(false);
        }

        craftingTableWindow.SetActive(false);
        CloseInventory();
    }
    public void CloseFurnace()
    {
        furnaceWindow.SetActive(false);
        CloseInventory();
    }

    public void OpenInventory()
    {
        inventoryWindow.SetActive(true);
        pm.playerState = PlayerState.OpenInventory;
        Cursor.visible = true;
    }
}
