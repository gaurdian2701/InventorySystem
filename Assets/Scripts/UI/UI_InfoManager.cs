using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoManager : MonoBehaviour
{
    [SerializeField] private GameObject itemInfoPanel;

    private GameObject inventoryPanel;
    private GameObject shopPanel;

    private InventoryService inventoryService;
    private ShopService shopService;

    private TextMeshProUGUI[] itemInfoPanelTexts; 
    private void Awake()
    {
        itemInfoPanel.SetActive(false);

        EventService.Instance.onItemUIClickedEvent.AddEventListener(PopUpInfoPanel);

        inventoryPanel = StorageService.Instance.GetInventoryPanel();
        shopPanel = StorageService.Instance.GetShopPanel();

        inventoryService = StorageService.Instance.inventoryService;
        shopService = StorageService.Instance.shopService;

        itemInfoPanelTexts = itemInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void PopUpInfoPanel(int layer, int itemIndex)
    {
        if (layer == inventoryPanel.layer)
            ShowInfo(inventoryService.inventoryItems[itemIndex]);
        else if (layer == shopPanel.layer)
            ShowInfo(shopService.shopItems[itemIndex]);
        else
            itemInfoPanel.SetActive(false);
    }

    private void ShowInfo(ItemScriptableObject itemInfo)
    {
        itemInfoPanel.SetActive(true);
        itemInfoPanelTexts[0].text = itemInfo.name.ToString();
        itemInfoPanelTexts[1].text = itemInfo.type.ToString();
        itemInfoPanelTexts[2].text = itemInfo.itemDescription.ToString();
        itemInfoPanelTexts[3].text = itemInfo.rarity.ToString();
        itemInfoPanelTexts[4].text = "Weight: " + itemInfo.weight.ToString();
        itemInfoPanelTexts[5].text = "Price: " + itemInfo.buyingPrice.ToString() + " Qty: " + itemInfo.quantity.ToString();
    }
}
