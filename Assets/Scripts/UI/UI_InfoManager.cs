using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoManager : MonoBehaviour
{
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI weightText;

    private GameObject inventoryPanel;
    private GameObject shopPanel;

    private InventoryService inventoryService;
    private ShopService shopService;

    private TextMeshProUGUI[] itemInfoPanelTexts;
    private void Awake()
    {
        itemInfoPanel.SetActive(false);

        EventService.Instance.onItemUIClickedEvent.AddEventListener(PopUpInfoPanel);
        EventService.Instance.onInventoryUpdated.AddEventListener(UpdateCurrencyAndWeight);

        inventoryPanel = StorageService.Instance.GetInventoryPanel();
        shopPanel = StorageService.Instance.GetShopPanel();

        inventoryService = StorageService.Instance.inventoryService;
        shopService = StorageService.Instance.shopService;

        itemInfoPanelTexts = itemInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
    }

    private void UpdateCurrencyAndWeight(int currency, float weight)
    {
        currencyText.text = currency.ToString();    
        weightText.text = weight.ToString();
    }

    private void PopUpInfoPanel(int layer, int itemIndex)
    {
        ItemScriptableObject item;
        if (layer == inventoryPanel.layer)
        {
            item = inventoryService.inventoryItems[itemIndex];
            ShowInfo(item, item.sellingPrice);
        }
        else if (layer == shopPanel.layer)
        {
            item = shopService.shopItems[itemIndex];
            ShowInfo(item, item.buyingPrice);
        }
        else
            itemInfoPanel.SetActive(false);
    }

    private void ShowInfo(ItemScriptableObject itemInfo, int price)
    {
        itemInfoPanel.SetActive(true);
        itemInfoPanelTexts[0].text = itemInfo.name.ToString();
        itemInfoPanelTexts[1].text = itemInfo.type.ToString();
        itemInfoPanelTexts[2].text = itemInfo.itemDescription.ToString();
        itemInfoPanelTexts[3].text = itemInfo.rarity.ToString();
        itemInfoPanelTexts[4].text = "Weight: " + itemInfo.weight.ToString();
        itemInfoPanelTexts[5].text = "Price: " + price.ToString() + " Qty: " + itemInfo.quantity.ToString();
    }
}
