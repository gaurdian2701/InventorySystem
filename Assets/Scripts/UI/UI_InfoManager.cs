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
    [SerializeField] private TextMeshProUGUI transactionStatusText;

    private GameObject inventoryPanel;
    private GameObject shopPanel;

    private ItemScriptableObject currentItemSelected;
    private int itemAmountSelected;
    private int playerCoinsOwned;

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

        inventoryPanel = StorageController.Instance.GetInventoryPanel();
        shopPanel = StorageController.Instance.GetActivePanel();

        itemInfoPanelTexts = itemInfoPanel.GetComponentsInChildren<TextMeshProUGUI>();
        itemAmountSelected = 0;
        playerCoinsOwned = 0;

        ClearTransactionStatusText();
    }

    private void UpdateCurrencyAndWeight(int amountOwned, float weight)
    {
        playerCoinsOwned = amountOwned;
        currencyText.text = playerCoinsOwned.ToString();
        weightText.text = weight.ToString();
    }

    private bool CanBuy()
    {
        if (playerCoinsOwned < currentItemSelected.buyingPrice * itemAmountSelected)
            return false;
        return true;
    }
    private void PopUpInfoPanel(int layer, int itemIndex)
    {
        if (buySellPanel.activeInHierarchy)
            CloseBuySellPanel();

        ItemScriptableObject item;
        if (layer == inventoryPanel.layer)
        {
            currentAction = CurrentAction.SELL;
            item = StorageController.Instance.GetInventoryService().inventoryItems[itemIndex];
            ShowInfo(item, item.sellingPrice);
        }
        else if (layer == shopPanel.layer)
        {
            currentAction = CurrentAction.BUY;
            item = StorageController.Instance.GetShopService().currentShopItemList[itemIndex];
            ShowInfo(item, item.buyingPrice);
        }
        else
        {
            currentItemSelected = null;
            itemInfoPanel.SetActive(false);
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
        OpenBuySellPanel();
    }

    public void ShowWeaponsPanel()
    {
        StorageController.Instance.SetActivePanel(ItemType.Weapon);
    }

    public void ShowConsumablesPanel()
    {
        StorageController.Instance.SetActivePanel(ItemType.Consumable);
    }

    public void ShowTreasuresPanel()
    {
        StorageController.Instance.SetActivePanel(ItemType.Treasure);
    }

    public void ShowMaterialsPanel()
    {
        StorageController.Instance.SetActivePanel(ItemType.Material);
    }

    public void ShowAllPanel()
    {
        StorageController.Instance.SetActivePanel(ItemType.None);
    }

    public void UpdateByIncrease()
    {
        if (itemAmountSelected >= currentItemSelected.quantity)
            return;

        int price = currentAction == CurrentAction.BUY ? currentItemSelected.buyingPrice : currentItemSelected.sellingPrice;
        buySellInfoText.text = $"{++itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * price} coins";
    }
  
    public void UpdateByDecrease()
    {
        if (itemAmountSelected <= 1)
            return;

        int price = currentAction == CurrentAction.BUY ? currentItemSelected.buyingPrice : currentItemSelected.sellingPrice;
        buySellInfoText.text = $"{--itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * price} coins";
    }

    public void ConfirmPayment()
    {

        if (currentAction == CurrentAction.SELL)
        {
            DisplayTransactionStatusMessage($"SOLD {itemAmountSelected} {currentItemSelected.name} for {currentItemSelected.sellingPrice * itemAmountSelected}");
            StorageController.Instance.DoSellTransaction(currentItemSelected, itemAmountSelected);
            //inventoryService.RemoveItemFromInventory(currentItemSelected, itemAmountSelected);
            //shopService.AddItemToShop(itemToBeAdded, itemAmountSelected);
        }

        else if(currentAction == CurrentAction.BUY)
        {
            if(!CanBuy())
            {
                DisplayTransactionStatusMessage("NOT ENOUGH MONEY!");
                return;
            }

            Debug.Log("itemamountselected: " + itemAmountSelected);
            DisplayTransactionStatusMessage($"BOUGHT {itemAmountSelected} {currentItemSelected.name} for {currentItemSelected.buyingPrice * itemAmountSelected}");
            StorageController.Instance.DoBuyTransaction(currentItemSelected, itemAmountSelected);
            //shopService.RemoveItemFromShop(currentItemSelected, itemAmountSelected);
            //inventoryService.AddItemToInventory(itemToBeAdded, itemAmountSelected);
        }

        CloseBuySellPanel();
        itemInfoPanel.SetActive(false);
    }

    private void DisplayTransactionStatusMessage(string message)
    {
        transactionStatusText.text = message;
        if (IsInvoking(nameof(ClearTransactionStatusText)))
            return;

        Invoke(nameof(ClearTransactionStatusText), 3f);
    }
    private void ClearTransactionStatusText() => transactionStatusText.text = string.Empty;
    private void CloseBuySellPanel()
    {
        buySellPanel.SetActive(false);
        itemAmountSelected = 0;
    }

    private void OpenBuySellPanel()
    {
        buySellPanel.SetActive(true);
        UpdateByIncrease();
        buySellInfoHeading.text = $"HOW MANY DO YOU WANT TO {currentAction}?";
    }
}
