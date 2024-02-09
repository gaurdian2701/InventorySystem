using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryService
{
    private GameObject inventoryPanel;
    private int itemLimit;
    private float weightLimit;

    private StorageUI inventoryUI;
    public List<ItemScriptableObject> inventoryItems {  get; private set; }

    public InventoryService(InventoryScriptableObject inventorySO, GameObject _inventoryPanel, string dataLoadPath)
    {
        inventoryPanel = _inventoryPanel;
        itemLimit = inventorySO.itemLimit;
        weightLimit = inventorySO.weightLimit;
        LoadData(dataLoadPath);
    }
    private void LoadData(string dataPath)
    {
        var emptyItem = Resources.LoadAll<ItemScriptableObject>(dataPath);
        inventoryItems = new List<ItemScriptableObject>(); //creating an empty list for inventory since we start out with nothing
        inventoryItems.Capacity = itemLimit;

        InitializeInventoryUI(emptyItem);
    }

    private void InitializeInventoryUI(ItemScriptableObject[] itemList)
    {
        var itemsUIList = new List<ItemScriptableObject>(itemList);
        inventoryUI = new StorageUI(inventoryPanel, itemsUIList);
    }
}
