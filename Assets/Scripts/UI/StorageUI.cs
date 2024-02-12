using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class StorageUI
{
    public GameObject storagePanel;
    public List<GameObject> itemsInStorage;

    public StorageUI(GameObject _StoragePanel, List<ItemScriptableObject> itemDataList)
    {
        storagePanel = _StoragePanel;
        itemsInStorage = new List<GameObject>();

        InitializeStorageUI(itemDataList);
    }

    private void InitializeStorageUI(List<ItemScriptableObject> itemData)
    {
        var itemDataList = itemData;

        foreach (var item in itemDataList)
            AddItemToStorageUI(item, item.quantity, int.MinValue);
    }

    public void AddItemToStorageUI(ItemScriptableObject item, int quantity, int index)
    {
        if (item.quantity == 0)
            return;

        if (index < 0)
        {
            GameObject itemUIElement = GameObject.Instantiate(item.itemUIPrefab, storagePanel.transform);
            itemUIElement.transform.GetChild((int)ItemPanelComponents.Icon).GetComponent<Image>().sprite = item.itemIcon;
            itemUIElement.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>().text = quantity.ToString();

            itemUIElement.name = item.name;
            itemsInStorage.Add(itemUIElement);
        }
        else
            UpdateItemQuantity(index, quantity);
    }

    private void UpdateItemQuantity(int itemIndex, int amount)
    {
        if (itemIndex < 0)
            return;

        GameObject itemUpdated = itemsInStorage[itemIndex];
        TextMeshProUGUI textComponent = itemUpdated.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>();

        string updateText = textComponent.text;
        int quantity = int.Parse(updateText);
        quantity += amount;

        updateText = quantity.ToString();
        textComponent.text = updateText;
    }

    public void RemoveItemFromStorageUI(ItemScriptableObject item, int quantity, int itemIndex)
    {
        Debug.Log("Item quantity: " + item.quantity + " Minus quantity: " + quantity);
        GameObject itemRemoved = itemsInStorage[itemIndex];

        if (!itemRemoved)
            return;

        if (item.quantity == 0)
        {
            itemsInStorage.Remove(itemRemoved);
            GameObject.Destroy(itemRemoved);
        }
        else
            UpdateItemQuantity(itemIndex, -quantity);
    }
}
