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
