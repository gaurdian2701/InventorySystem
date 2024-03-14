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
    public List<ItemScriptableObject> InventoryItems { get; private set; }

    public InventoryService(InventoryScriptableObject inventorySO, GameObject _inventoryPanel, string dataLoadPath)
    {
        inventoryPanel = _inventoryPanel;
        itemLimit = inventorySO.ItemLimit;
        weightLimit = inventorySO.WeightLimit;

        currentWeight = 0f;
        coinsOwned = 0;

        isGathering = false;

        LoadData(dataLoadPath);
    }
    private void LoadData(string dataPath)
    {
        var emptyItem = Resources.LoadAll<ItemScriptableObject>(dataPath);
        InventoryItems = new List<ItemScriptableObject>(); //creating an empty list for inventory since we start out with nothing
        InventoryItems.Capacity = itemLimit;

        InitializeInventoryUI(emptyItem);
    }

    private void InitializeInventoryUI(ItemScriptableObject[] itemList)
    {
        var itemsUIList = new List<ItemScriptableObject>(itemList);
        inventoryUI = new StorageUI(inventoryPanel, itemsUIList);
    }

    public void AddItemToInventory(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = InventoryItems.Find((x) => x.name == item.name);
        int index = int.MinValue;

        if (!itemFound)
        {
            InventoryItems.Add(item);
            item.Quantity = quantity;
        }

        else
        {
            itemFound.Quantity += quantity;
            index = InventoryItems.IndexOf(itemFound);
        }

        inventoryUI.AddItemToStorageUI(item, quantity, index);
        currentWeight += item.Weight * quantity;

        if(!isGathering)
            coinsOwned -= item.BuyingPrice * quantity;

        if(coinsOwned < 0)
            coinsOwned = 0;

        EventService.Instance.OnInventoryUpdated.InvokeEvent(coinsOwned, currentWeight);
    }

    public void RemoveItemFromInventory(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = InventoryItems.Find((x) => x.name == item.name);

        if (!itemFound || !CanRemoveItems(itemFound, quantity))
            return;

        int index = InventoryItems.IndexOf(itemFound);

        currentWeight -= itemFound.Weight * quantity;
        coinsOwned += itemFound.SellingPrice * quantity;

        if (itemFound.Quantity - quantity == 0)
        {
            itemFound.Quantity = 0;
            InventoryItems.Remove(itemFound);
        }
        else
            itemFound.Quantity -= quantity;

        inventoryUI.RemoveItemFromStorageUI(itemFound, quantity, index);
        EventService.Instance.OnInventoryUpdated?.InvokeEvent(coinsOwned, currentWeight);
    }

    public void FillInventory()
    {
        var resourcesGathered = Resources.LoadAll<ItemScriptableObject>("ItemSOs/ResourceGathering");

        isGathering = true;

        foreach (var item in resourcesGathered)
        {
            var newItem = GameObject.Instantiate<ItemScriptableObject>(item);
            newItem.name = item.name;

            if (!HasEnoughWeight(newItem, newItem.Quantity))
            {
                EventService.Instance.OnItemAdditionFailure.InvokeEvent(ItemAdditionFailureType.WEIGHT);
                break;
            }
            
            AddItemToInventory(newItem, newItem.Quantity);
        }

        isGathering = false;
    }

    public bool HasEnoughWeight(ItemScriptableObject item, int quantity)
    {
        if (currentWeight + item.Weight * quantity > weightLimit)
            return false;

        return true;
    }

    public bool HasEnoughCoins(ItemScriptableObject item, int quantity)
    {
        if (coinsOwned < item.BuyingPrice * quantity)
            return false;

        return true;
    }

    private bool CanRemoveItems(ItemScriptableObject item, int quantity)
    {
        if(item.Quantity < quantity)
            return false;
        return true;
    }
}
