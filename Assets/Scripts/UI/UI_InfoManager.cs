using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_InfoManager : MonoBehaviour
{
    [Header("UI Panels")]
    [SerializeField] private GameObject itemInfoPanel;
    [SerializeField] private GameObject buySellPanel;

    [Header("Text Info")]
    [SerializeField] private TextMeshProUGUI currencyText;
    [SerializeField] private TextMeshProUGUI weightText;
    [SerializeField] private TextMeshProUGUI buySellInfoHeading;
    [SerializeField] private TextMeshProUGUI buySellInfoText;
    [SerializeField] private TextMeshProUGUI buySellButtonText;

    private GameObject inventoryPanel;
    private GameObject shopPanel;

    private InventoryService inventoryService;
    private ShopService shopService;

    private ItemScriptableObject currentItemSelected;
    private int itemAmountSelected;

    private TextMeshProUGUI[] itemInfoPanelTexts;

    private enum CurrentAction
    {
        BUY,
        SELL
    };

    private CurrentAction currentAction;

    private void Awake()
    {
        itemInfoPanel.SetActive(false);
        buySellPanel.SetActive(false);

        EventService.Instance.onItemUIClickedEvent.AddEventListener(PopUpInfoPanel);
        EventService.Instance.onInventoryUpdated.AddEventListener(UpdateCurrencyAndWeight);

        inventoryPanel = StorageService.Instance.GetInventoryPanel();
        shopPanel = StorageService.Instance.GetShopPanel();

        inventoryService = StorageService.Instance.inventoryService;
        shopService = StorageService.Instance.shopService;

        itemInfoPanelTexts = itemInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
        itemAmountSelected = 0;
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
            currentAction = CurrentAction.SELL;
            item = inventoryService.inventoryItems[itemIndex];
            ShowInfo(item, item.sellingPrice);
        }
        else if (layer == shopPanel.layer)
        {
            currentAction = CurrentAction.BUY;
            item = shopService.shopItems[itemIndex];
            ShowInfo(item, item.buyingPrice);
        }
        else
        {
            if (buySellPanel.activeInHierarchy)
                buySellPanel.SetActive(false);

            itemInfoPanel.SetActive(false);
            currentItemSelected = null;
            itemAmountSelected = 0;
        }
    }

    private void ShowInfo(ItemScriptableObject itemInfo, int price)
    {
        buySellButtonText.text = currentAction.ToString();
        currentItemSelected = itemInfo;

        itemInfoPanel.SetActive(true);
        itemInfoPanelTexts[0].text = itemInfo.name.ToString();
        itemInfoPanelTexts[1].text = itemInfo.type.ToString();
        itemInfoPanelTexts[2].text = itemInfo.itemDescription.ToString();
        itemInfoPanelTexts[3].text = itemInfo.rarity.ToString();
        itemInfoPanelTexts[4].text = "Weight: " + itemInfo.weight.ToString();
        itemInfoPanelTexts[5].text = "Price: " + price.ToString() + " Qty: " + itemInfo.quantity.ToString();
    }

    public void ShowBuySellPanel()
    {
        buySellPanel.SetActive(true);
        buySellInfoHeading.text = $"HOW MANY DO YOU WANT TO {currentAction}?";
        UpdateByIncrease();
    }

    public void UpdateByIncrease()
    {
        if (itemAmountSelected >= currentItemSelected.quantity)
            return;

        buySellInfoText.text = $"{++itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * currentItemSelected.sellingPrice} coins";
    }
  
    public void UpdateByDecrease()
    {
        if (itemAmountSelected <= 1)
            return;

        buySellInfoText.text = $"{--itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * currentItemSelected.sellingPrice} coins";
    }
}
