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
    private int coinsOwned;

    private bool isGathering;

    private StorageUI inventoryUI;
    public List<ItemScriptableObject> inventoryItems { get; private set; }

    public InventoryService(InventoryScriptableObject inventorySO, GameObject _inventoryPanel, string dataLoadPath)
    {
        inventoryPanel = _inventoryPanel;
        itemLimit = inventorySO.itemLimit;
        weightLimit = inventorySO.weightLimit;

        currentWeight = 0f;
        coinsOwned = 0;

        isGathering = false;

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

    public void AddItemToInventory(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = inventoryItems.Find((x) => x.name == item.name);
        int index = int.MinValue;

        if (!itemFound)
        {
            inventoryItems.Add(item);
            item.quantity = quantity;
        }

        else
        {
            itemFound.quantity += quantity;
            index = inventoryItems.IndexOf(itemFound);
        }

        inventoryUI.AddItemToStorageUI(item, quantity, index);
        currentWeight += item.weight * quantity;

        if(!isGathering)
            coinsOwned -= item.buyingPrice * quantity;

        if(coinsOwned < 0)
            coinsOwned = 0;

        EventService.Instance.onInventoryUpdated.InvokeEvent(coinsOwned, currentWeight);
    }

    public void RemoveItemFromInventory(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = inventoryItems.Find((x) => x.name == item.name);

        if (!itemFound || !CanRemoveItems(itemFound, quantity))
            return;

        int index = inventoryItems.IndexOf(itemFound);

        currentWeight -= itemFound.weight * quantity;
        coinsOwned += itemFound.sellingPrice * quantity;

        if (itemFound.quantity - quantity == 0)
        {
            itemFound.quantity = 0;
            inventoryItems.Remove(itemFound);
        }
        else
            itemFound.quantity -= quantity;

        inventoryUI.RemoveItemFromStorageUI(itemFound, quantity, index);
        EventService.Instance.onInventoryUpdated?.InvokeEvent(coinsOwned, currentWeight);
    }

    public void FillInventory()
    {
        var resourcesGathered = Resources.LoadAll<ItemScriptableObject>("ItemSOs/ResourceGathering");

        isGathering = true;

        foreach (var item in resourcesGathered)
        {
            var newItem = GameObject.Instantiate<ItemScriptableObject>(item);
            newItem.name = item.name;

            if (!HasEnoughWeight(newItem, newItem.quantity))
            {
                EventService.Instance.onItemAdditionFailure.InvokeEvent(ItemAdditionFailureType.WEIGHT);
                break;
            }
            
            AddItemToInventory(newItem, newItem.quantity);
        }

        isGathering = false;
    }

    public bool HasEnoughWeight(ItemScriptableObject item, int quantity)
    {
        if (currentWeight + item.weight * quantity > weightLimit)
            return false;

        return true;
    }

    public bool HasEnoughCoins(ItemScriptableObject item, int quantity)
    {
        if (coinsOwned < item.buyingPrice * quantity)
            return false;

        return true;
    }

    private bool CanRemoveItems(ItemScriptableObject item, int quantity)
    {
        if(item.quantity < quantity)
            return false;
        return true;
    }
}
