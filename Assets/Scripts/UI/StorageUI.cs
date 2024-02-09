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
            AddItemToStorage(item);
    }

    public void AddItemToStorage(ItemScriptableObject item)
    {
        if (item.quantity == 0)
            return;

        GameObject itemUIElement = GameObject.Instantiate(item.itemUIPrefab, storagePanel.transform);
        itemUIElement.transform.GetChild((int)ItemPanelComponents.Icon).GetComponent<Image>().sprite = item.itemIcon;
        itemUIElement.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>().text = item.quantity.ToString();

        itemUIElement.name = item.name;

        itemsInStorage.Add(itemUIElement);
    }

    public void UpdateItemQuantity(int itemIndex, int amount)
    {
        GameObject itemUpdated = itemsInStorage[itemIndex];
        TextMeshProUGUI textComponent = itemUpdated.transform.GetChild((int)ItemPanelComponents.Quantity).GetComponent<TextMeshProUGUI>();

        string updateText = textComponent.text;
        int quantity = int.Parse(updateText);
        quantity += amount;

        updateText = quantity.ToString();
        textComponent.text = updateText;
    }
}