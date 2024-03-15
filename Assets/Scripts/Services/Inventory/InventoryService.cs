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
    private bool isGathering;
    private StorageUI inventoryUI;
    private CurrencyController currencyController;
    private const int startingCoins = 0;
    private const string resourcesPath = "ItemSOs/ResourceGathering";
    public List<ItemScriptableObject> InventoryItems { get; private set; }

    public InventoryService(InventoryScriptableObject inventorySO, GameObject _inventoryPanel, string dataLoadPath)
    {
        inventoryPanel = _inventoryPanel;
        itemLimit = inventorySO.ItemLimit;
        weightLimit = inventorySO.WeightLimit;

        currentWeight = 0f;
        currencyController = new CurrencyController(startingCoins);

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
        ItemScriptableObject itemFound = null;
        int itemIndex = -1;
        bool itemHasBeenFound = false;

        for (int i = 0; i < InventoryItems.Count; i++)
            if (InventoryItems[i].name == item.name)
            {
                itemIndex = i;
                itemHasBeenFound = true;
                itemFound = InventoryItems[i];
            }

        if (!itemHasBeenFound)
        {
            InventoryItems.Add(item);
            item.Quantity = quantity;
        }
        else
            itemFound.Quantity += quantity;

        inventoryUI.AddItemToStorageUI(item, quantity, itemIndex, itemHasBeenFound);
        currentWeight += item.Weight * quantity;

        if (!isGathering)
            currencyController.UpdateCoins(-item.BuyingPrice * quantity);

        if(currencyController.CoinsOwned < 0)
            currencyController.UpdateCoins(0);

        GameService.Instance.EventService.OnInventoryUpdated.Invoke(currencyController.CoinsOwned, currentWeight);
    }

    public void RemoveItemFromInventory(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = null;
        int itemIndex = -1;
        for(int i = 0; i < InventoryItems.Count; i++)
            if (InventoryItems[i].name == item.name)
            {
                itemFound = InventoryItems[i];
                itemIndex = i;
            }

        if (itemFound == null || !CanRemoveItems(itemFound, quantity))
            return;

        currentWeight -= itemFound.Weight * quantity;
        currencyController.UpdateCoins(itemFound.SellingPrice * quantity);

        if (itemFound.Quantity - quantity == 0)
        {
            itemFound.Quantity = 0;
            InventoryItems.Remove(itemFound);
        }
        else
            itemFound.Quantity -= quantity;

        inventoryUI.RemoveItemFromStorageUI(itemFound, quantity, itemIndex);
        GameService.Instance.EventService.OnInventoryUpdated.Invoke(currencyController.CoinsOwned, currentWeight);
    }

    public void FillInventory()
    {
        var resourcesGathered = Resources.LoadAll<ItemScriptableObject>(resourcesPath);

        isGathering = true;

        foreach (var item in resourcesGathered)
        {
            var newItem = GameObject.Instantiate<ItemScriptableObject>(item);
            newItem.name = item.name;

            if (!HasEnoughWeight(newItem, newItem.Quantity))
            {
                GameService.Instance.EventService.OnItemAdditionFailure.Invoke(ItemAdditionFailureType.WEIGHT);
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
        if (currencyController.CoinsOwned < item.BuyingPrice * quantity)
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
