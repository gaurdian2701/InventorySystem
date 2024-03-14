using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI
{
    public GameObject StoragePanel;
    public List<GameObject> ItemsInStorage;

    private const int defaultIndex = -1;

    public StorageUI(GameObject _StoragePanel, List<ItemScriptableObject> itemDataList)
    {
        StoragePanel = _StoragePanel;
        ItemsInStorage = new List<GameObject>();

        InitializeStorageUI(itemDataList);
    }

    private void InitializeStorageUI(List<ItemScriptableObject> itemData) //Fill corresponding UI panel with items
    {
        var itemDataList = itemData;

        foreach (var item in itemDataList)
            AddItemToStorageUI(item, item.Quantity, defaultIndex, false);
    }

    public void AddItemToStorageUI(ItemScriptableObject item, int quantity, int index, bool itemAlreadyExists)
    {
        if (item.Quantity == 0)
            return;

        if (!itemAlreadyExists)
        {
            GameObject itemUIElement = GameObject.Instantiate(item.ItemUIPrefab, StoragePanel.transform);
            itemUIElement.transform.GetChild((int)ItemPanelComponents.Icon).GetComponent<Image>().sprite = item.ItemIcon;
            itemUIElement.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>().text = quantity.ToString();

            itemUIElement.name = item.name;
            ItemsInStorage.Add(itemUIElement);
        }
        else
            UpdateItemQuantity(index, quantity, itemAlreadyExists);
    }

    private void UpdateItemQuantity(int itemIndex, int amount, bool itemAlreadyExists) 
    {
        if (!itemAlreadyExists)
            return;

        GameObject itemUpdated = ItemsInStorage[itemIndex];
        TextMeshProUGUI textComponent = itemUpdated.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>();

        string updateText = textComponent.text;
        int quantity = int.Parse(updateText);
        quantity += amount;

        updateText = quantity.ToString();
        textComponent.text = updateText;
    }

    public void RemoveItemFromStorageUI(ItemScriptableObject item, int quantity, int itemIndex)
    {
        GameObject itemRemoved = ItemsInStorage[itemIndex];

        if (!itemRemoved)
            return;

        if (item.Quantity == 0)
        {
            ItemsInStorage.Remove(itemRemoved);
            GameObject.Destroy(itemRemoved);
        }
        else
            UpdateItemQuantity(itemIndex, -quantity, true);
    }
}
