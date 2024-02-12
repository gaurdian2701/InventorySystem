using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using UnityEngine;

public class ShopService
{
    private GameObject mainShopPanel;
    private GameObject weaponsPanel;
    private GameObject consumablesPanel;
    private GameObject treasuresPanel;
    private GameObject materialsPanel;

    private StorageUI mainShopUI;
    private StorageUI weaponsUI;
    private StorageUI consumablesUI;
    private StorageUI treasuresUI;
    private StorageUI materialsUI;

    public List<ItemScriptableObject> currentShopItemList { get; private set; }

    public List<ItemScriptableObject> mainShopList { get; private set; }
    public List<ItemScriptableObject> weaponsList { get; private set; }
    public List<ItemScriptableObject> consumablesList { get; private set; }
    public List<ItemScriptableObject> treasuresList { get; private set; }
    public List<ItemScriptableObject> materialsList { get; private set; }


    public ShopService(
        GameObject _mainShopPanel,
        GameObject _weaponsPanel,
        GameObject _consumablesPanel,
        GameObject _treasuresPanel,
        GameObject _materialsPanel,
        string dataLoadPath)
    {
        mainShopPanel = _mainShopPanel;
        weaponsPanel = _weaponsPanel;
        consumablesPanel = _consumablesPanel;
        treasuresPanel = _treasuresPanel;
        materialsPanel = _materialsPanel;

        mainShopList = new List<ItemScriptableObject>();
        weaponsList = new List<ItemScriptableObject>();
        consumablesList = new List<ItemScriptableObject>();
        treasuresList = new List<ItemScriptableObject>();
        materialsList = new List<ItemScriptableObject>();
        currentShopItemList = new List<ItemScriptableObject>();

        currentShopItemList = mainShopList;
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
            AddItemToLists(newItem, newItem.quantity);
        }

        mainShopUI = new StorageUI(mainShopPanel, mainShopList);
        weaponsUI = new StorageUI(weaponsPanel, weaponsList);
        consumablesUI = new StorageUI(consumablesPanel, consumablesList);
        treasuresUI = new StorageUI(treasuresPanel, treasuresList);
        materialsUI = new StorageUI(materialsPanel, materialsList);
    }

    public void SetCurrentList(ItemType listType)
    {
        switch(listType)
        {
            case ItemType.Weapon:
                currentShopItemList = weaponsList; break;

            case ItemType.Consumable:
                currentShopItemList = consumablesList; break;

            case ItemType.Treasure:
                currentShopItemList = treasuresList; break;

            case ItemType.Material:
                currentShopItemList = materialsList; break;

            default: 
                currentShopItemList = mainShopList; break;
        }
    }
    public void AddItemToShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = mainShopList.Find((x) => x.name == item.name);
        int mainIndex = int.MinValue;
        int typeIndex = int.MinValue;

        if (!itemFound)
            AddItemToLists(item, quantity);

        else
        {
            mainIndex = mainShopList.IndexOf(itemFound);
            itemFound.quantity += quantity;
            UpdateItemDataInOtherLists(item, out typeIndex);
        }

        mainShopUI.AddItemToStorageUI(item, quantity, mainIndex);
        AddItemToTypeUI(item, quantity, typeIndex);
    }

    private void AddItemToLists(ItemScriptableObject item, int quantity)
    {
        mainShopList.Add(item);

        switch (item.type)
        {
            case ItemType.Weapon:
                Debug.Log("item added to weapons list");
                weaponsList.Add(item);
                break;

            case ItemType.Consumable:
                Debug.Log("item added to consumables list");
                consumablesList.Add(item);
                break;

            case ItemType.Treasure:
                Debug.Log("item added to treasures list");
                treasuresList.Add(item);
                break;

            case ItemType.Material:
                Debug.Log("item added to materials list");
                materialsList.Add(item);
                break;

            default: break;
        }

        item.quantity = quantity;
    }

    private void UpdateItemDataInOtherLists(ItemScriptableObject itemInMainList, out int index)
    {
        index = -1;

        switch (itemInMainList.type)
        {
            case ItemType.Weapon:
                index = weaponsList.IndexOf(itemInMainList);
                Debug.Log("weapons list updated");
                break;

            case ItemType.Consumable:
                index = consumablesList.IndexOf(itemInMainList);
                Debug.Log("consumables list updated");
                break;

            case ItemType.Treasure:
                index = treasuresList.IndexOf(itemInMainList);
                Debug.Log("treasures list updated");
                break;

            case ItemType.Material:
                index = materialsList.IndexOf(itemInMainList);
                Debug.Log("materials list updated");
                break;

            default: break;
        }
    }

    private void AddItemToTypeUI(ItemScriptableObject item, int quantity, int index)
    {
        switch (item.type)
        {
            case ItemType.Weapon:
                Debug.Log("UI for weapons updated");
                weaponsUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Consumable:
                Debug.Log("UI for consumables updated");
                consumablesUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Treasure:
                Debug.Log("UI for treasures updated");
                treasuresUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Material:
                Debug.Log("UI for materials updated");
                materialsUI.AddItemToStorageUI(item, quantity, index);
                break;
            default: break;
        }
    }

    public void RemoveItemFromShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = mainShopList.Find((x) => x.name == item.name);

        if (!itemFound || itemFound.quantity < quantity)
            return;

        int mainIndex = mainShopList.IndexOf(itemFound);
        int typeIndex = -1;

        if (itemFound.quantity - quantity == 0)
        {
            itemFound.quantity = 0;
            RemoveItemFromOtherLists(itemFound, out typeIndex);
            mainShopList.Remove(itemFound);
        }
        else
            itemFound.quantity -= quantity;

        mainShopUI.RemoveItemFromStorageUI(itemFound, quantity, mainIndex);
        RemoveItemFromTypeUI(itemFound, quantity, typeIndex);
    }

    private void RemoveItemFromOtherLists(ItemScriptableObject item, out int index)
    {
        index = -1;
        switch (item.type)
        {
            case ItemType.Weapon:
                Debug.Log("item removed from weapons list");
                weaponsList.Remove(item);
                index = weaponsList.IndexOf(item);
                break;

            case ItemType.Consumable:
                Debug.Log("item removed from consumables list");
                index = weaponsList.IndexOf(item);
                consumablesList.Remove(item);
                break;

            case ItemType.Treasure:
                Debug.Log("item removed from treasures list");
                treasuresList.Remove(item);
                index = treasuresList.IndexOf(item);
                break;

            case ItemType.Material:
                Debug.Log("item removed from materials list");
                materialsList.Remove(item);
                index = materialsList.IndexOf(item);
                break;

            default: break;
        }
    }

    private void RemoveItemFromTypeUI(ItemScriptableObject item, int quantity, int index)
    {
        switch (item.type)
        {
            case ItemType.Weapon:
                Debug.Log("item removed from weapons UI");
                weaponsUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            case ItemType.Consumable:
                Debug.Log("item removed from consumables UI");
                consumablesUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            case ItemType.Treasure:
                Debug.Log("item removed from treasures UI");
                treasuresUI.RemoveItemFromStorageUI(item, index, quantity);
                break;

            case ItemType.Material:
                Debug.Log("item removed from materials UI");
                materialsUI.RemoveItemFromStorageUI(item, index, quantity);
                break;

            default: break;
        }
    }
}
