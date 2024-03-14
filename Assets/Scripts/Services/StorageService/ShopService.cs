using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Reflection;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
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

    public List<ItemScriptableObject> CurrentShopItemList { get; private set; }

    public List<ItemScriptableObject> MainShopList { get; private set; }
    public List<ItemScriptableObject> WeaponsList { get; private set; }
    public List<ItemScriptableObject> ConsumablesList { get; private set; }
    public List<ItemScriptableObject> TreasuresList { get; private set; }
    public List<ItemScriptableObject> MaterialsList { get; private set; }


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

        MainShopList = new List<ItemScriptableObject>();
        WeaponsList = new List<ItemScriptableObject>();
        ConsumablesList = new List<ItemScriptableObject>();
        TreasuresList = new List<ItemScriptableObject>();
        MaterialsList = new List<ItemScriptableObject>();
        CurrentShopItemList = new List<ItemScriptableObject>();

        CurrentShopItemList = MainShopList;
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
            AddItemToLists(newItem, newItem.Quantity);
        }

        mainShopUI = new StorageUI(mainShopPanel, MainShopList);
        weaponsUI = new StorageUI(weaponsPanel, WeaponsList);
        consumablesUI = new StorageUI(consumablesPanel, ConsumablesList);
        treasuresUI = new StorageUI(treasuresPanel, TreasuresList);
        materialsUI = new StorageUI(materialsPanel, MaterialsList); //Creating StorageUI instances for each type panel
    }

    //The currentShopItemList variable is used by the UI_InfoManager to read data belonging to the current active panel, 
    //i.e.(All, Weapons, Consumables, etc.)
    //This function is used to dynamically change the currentShopItemList to the type list corresponding to the panel selected
    public void SetCurrentList(ItemType listType)
    {
        switch(listType)
        {
            case ItemType.Weapon:
                CurrentShopItemList = WeaponsList; break;

            case ItemType.Consumable:
                CurrentShopItemList = ConsumablesList; break;

            case ItemType.Treasure:
                CurrentShopItemList = TreasuresList; break;

            case ItemType.Material:
                CurrentShopItemList = MaterialsList; break;

            default: 
                CurrentShopItemList = MainShopList; break;
        }
    }
    public void AddItemToShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = MainShopList.Find((x) => x.name == item.name);
        int mainIndex = int.MinValue;
        int typeIndex = int.MinValue;

        if (!itemFound)
            AddItemToLists(item, quantity);

        else
        {
            mainIndex = MainShopList.IndexOf(itemFound);
            itemFound.Quantity += quantity;
            FindItemInOtherLists(itemFound, out typeIndex);
        }

        mainShopUI.AddItemToStorageUI(item, quantity, mainIndex);
        AddItemToTypeUI(item, quantity, typeIndex);
    }

    private void AddItemToLists(ItemScriptableObject item, int quantity) //Adds the item to the main list as well as the list corresponding to its type
    {
        MainShopList.Add(item);

        switch (item.Type)
        {
            case ItemType.Weapon:
                WeaponsList.Add(item);
                break;

            case ItemType.Consumable:
                ConsumablesList.Add(item);
                break;

            case ItemType.Treasure:
                TreasuresList.Add(item);
                break;

            case ItemType.Material:
                MaterialsList.Add(item);
                break;

            default: break;
        }

        item.Quantity = quantity;
    }
    private void AddItemToTypeUI(ItemScriptableObject item, int quantity, int index) //Adds the item to the corresponding type panel
    {
        switch (item.Type)
        {
            case ItemType.Weapon:
                weaponsUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Consumable:
                consumablesUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Treasure:
                treasuresUI.AddItemToStorageUI(item, quantity, index);
                break;
            case ItemType.Material:
                materialsUI.AddItemToStorageUI(item, quantity, index);
                break;
            default: break;
        }
    }

    public void RemoveItemFromShop(ItemScriptableObject item, int quantity)
    {
        ItemScriptableObject itemFound = MainShopList.Find((x) => x.name == item.name);

        if (!itemFound || itemFound.Quantity < quantity)
            return;

        int mainIndex = MainShopList.IndexOf(itemFound);
        int typeIndex = 0;
        var typeList = FindItemInOtherLists(itemFound, out typeIndex);

        if (itemFound.Quantity - quantity == 0)
        {
            itemFound.Quantity = 0;

            typeList.Remove(itemFound);
            MainShopList.Remove(itemFound);
        }
        else
            itemFound.Quantity -= quantity;

        mainShopUI.RemoveItemFromStorageUI(itemFound, quantity, mainIndex);
        RemoveItemFromTypeUI(itemFound, quantity, typeIndex);
    }

    private List<ItemScriptableObject> FindItemInOtherLists(ItemScriptableObject item, out int index) //Finds the item in the corresponding type list
    {
        List<ItemScriptableObject> typeList = new List<ItemScriptableObject>();
        switch (item.Type)
        {
            case ItemType.Weapon:
                index = WeaponsList.IndexOf(item);
                typeList = WeaponsList;
                break;

            case ItemType.Consumable:
                index = ConsumablesList.IndexOf(item);
                typeList = ConsumablesList;
                break;

            case ItemType.Treasure:
                index = TreasuresList.IndexOf(item);
                typeList = TreasuresList;
                break;

            case ItemType.Material:
                index = MaterialsList.IndexOf(item);
                typeList = MaterialsList;
                break;

            default:
                index = -1;
                break;
        }
        return typeList;
    }

    private void RemoveItemFromTypeUI(ItemScriptableObject item, int quantity, int index) //Removes the item from the corresponding type panel
    {
        switch (item.Type)
        {
            case ItemType.Weapon:
                weaponsUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            case ItemType.Consumable:
                consumablesUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            case ItemType.Treasure:
                treasuresUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            case ItemType.Material:
                materialsUI.RemoveItemFromStorageUI(item, quantity, index);
                break;

            default: break;
        }
    }
}
