using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopService
{
    private GameObject shopPanel;
    private GameObject itemInfoPanel;
    private StorageUI shopUI;

    public List<ItemScriptableObject> shopItems { get; private set; }

    public ShopService(GameObject _shopPanel, string dataLoadPath)
    {
        shopPanel = _shopPanel;
        shopItems = new List<ItemScriptableObject>();
        LoadData(dataLoadPath);
    }

    public void AddItemToShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = shopItems.Find((x) => x.name == item.name);
        int index = int.MinValue;

        if (!itemFound)
        {
            shopItems.Add(item);
            item.quantity = quantity;
        }

        else
        {
            itemFound.quantity += quantity;
            index = shopItems.IndexOf(itemFound);
        }

        shopUI.AddItemToStorage(item, quantity, index);
    }

    public void RemoveItemFromShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = shopItems.Find((x) => x.name == item.name);

        if (!itemFound || itemFound.quantity < quantity)
            return;

        int index = shopItems.IndexOf(itemFound);
        shopUI.RemoveItemFromStorage(itemFound, index, quantity);

        if(itemFound.quantity - quantity == 0)
            shopItems.Remove(itemFound);
        else
            itemFound.quantity -= quantity;
    }
    private void LoadData(string dataLoadPath)
    {
        var shopItemsList = Resources.LoadAll<ItemScriptableObject>(dataLoadPath); //Loading all the shop items
        InitializeShopUI(shopItemsList);
    }

    private void InitializeShopUI(ItemScriptableObject[] shopItemsList)
    {
        foreach (var item in shopItemsList)
        {
            var newItem = GameObject.Instantiate(item);
            newItem.name = item.name;
            shopItems.Add(newItem);
        }
        shopUI = new StorageUI(shopPanel, shopItems);
    }
}
