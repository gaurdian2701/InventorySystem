using System;
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

    private float currentWeight;
    private int currentItemNumbers;

    private StorageUI inventoryUI;
    public List<ItemScriptableObject> inventoryItems {  get; private set; }

    public InventoryService(InventoryScriptableObject inventorySO, GameObject _inventoryPanel, string dataLoadPath)
    {
        inventoryPanel = _inventoryPanel;
        itemLimit = inventorySO.itemLimit;
        weightLimit = inventorySO.weightLimit;

        currentWeight = 0f;
        currentItemNumbers = 0;

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

    public void AddItemToInventory(ItemScriptableObject item)
    {
        if (!CanAddItems(item))
            return; //Add logic to show appropriate message

        ItemScriptableObject itemFound = inventoryItems.Find((x) => x.name == item.name);

        if (!itemFound)
        {
            inventoryItems.Add(item);
            inventoryUI.AddItemToStorage(item);
        }

        else
        {
            int index = inventoryItems.IndexOf(itemFound);
            itemFound.quantity += item.quantity;
            inventoryUI.UpdateItemQuantity(index, item.quantity);
        }
        currentWeight += item.weight;

        EventService.Instance.onInventoryUpdated.InvokeEvent(0, currentWeight);
    }

    public void FillInventory()
    {
        var resourcesGathered = Resources.LoadAll<ItemScriptableObject>("ItemSOs/ResourceGathering");
        
        foreach (var item in resourcesGathered)
        {
            var newItem = GameObject.Instantiate<ItemScriptableObject>(item);
            newItem.name = item.name;
            AddItemToInventory(newItem);
        }
    }

    private bool CanAddItems(ItemScriptableObject item)
    {
        if(currentWeight + item.weight > weightLimit)
            return false;
        return true;
    }
}
