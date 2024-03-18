using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UIService : MonoBehaviour
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

    [Header("Item Info Panel Text")]
    [SerializeField] private TextMeshProUGUI itemName;
    [SerializeField] private TextMeshProUGUI itemType;
    [SerializeField] private TextMeshProUGUI itemDescription;
    [SerializeField] private TextMeshProUGUI itemRarity;
    [SerializeField] private TextMeshProUGUI itemWeight;
    [SerializeField] private TextMeshProUGUI itemQtyAndPrice;

    private GameObject inventoryPanel;
    private GameObject shopPanel;
    private ItemScriptableObject currentItemSelected;
    private int itemAmountSelected;
    private const float transactionStatusTextDisplayTime = 3f;
    private bool transactionStatusTextAlreadyDisplayed = false;
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

        GameService.Instance.EventService.OnItemUIClickedEvent += PopUpInfoPanel;
        GameService.Instance.EventService.OnInventoryUpdated += UpdateCurrencyAndWeight;
        GameService.Instance.EventService.OnItemAdditionFailure += HandleItemAdditionFailure;

        inventoryPanel = GameService.Instance.StorageService.GetInventoryPanel();
        shopPanel = GameService.Instance.StorageService.GetActivePanel();
        itemAmountSelected = 0;
        EmptyTransactionStatusText();
    }

    private void OnDestroy()
    {
        GameService.Instance.EventService.OnItemUIClickedEvent -= PopUpInfoPanel;
        GameService.Instance.EventService.OnInventoryUpdated -= UpdateCurrencyAndWeight;
        GameService.Instance.EventService.OnItemAdditionFailure -= HandleItemAdditionFailure;
    }
    private void HandleItemAdditionFailure(ItemAdditionFailureType type)
    {
        DisplayTransactionStatusMessage($"NOT ENOUGH {type}!");
    }

    private void UpdateCurrencyAndWeight(int amountOwned, float weight)
    {
        currencyText.text = amountOwned.ToString();
        weightText.text = weight.ToString();
    }

    private void PopUpInfoPanel(int layer, int itemIndex)
    {
        if (buySellPanel.activeInHierarchy)
            CloseBuySellPanel();

        ItemScriptableObject item;
        if (layer == inventoryPanel.layer)
        {
            currentAction = CurrentAction.SELL;
            item = GameService.Instance.StorageService.GetInventoryService().InventoryItems[itemIndex];
            ShowInfo(item, item.SellingPrice);
        }
        else if (layer == shopPanel.layer)
        {
            currentAction = CurrentAction.BUY;
            item = GameService.Instance.StorageService.GetShopService().CurrentShopItemList[itemIndex];
            ShowInfo(item, item.BuyingPrice);
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
        itemName.text = itemInfo.name.ToString();
        itemType.text = itemInfo.Type.ToString();   
        itemDescription.text = itemInfo.ItemDescription.ToString();
        itemRarity.text = itemInfo.Rarity.ToString();
        itemWeight.text = "Weight: " + itemInfo.Weight.ToString();
        itemQtyAndPrice.text = "Price: " + price.ToString() + " Qty: " + itemInfo.Quantity.ToString();
    }

    public void ShowBuySellPanel()
    {
        OpenBuySellPanel();
    }

    public void ShowWeaponsPanel()
    {
        GameService.Instance.StorageService.SetActivePanel(ItemType.Weapon);
    }

    public void ShowConsumablesPanel()
    {
        GameService.Instance.StorageService.SetActivePanel(ItemType.Consumable);
    }

    public void ShowTreasuresPanel()
    {
        GameService.Instance.StorageService.SetActivePanel(ItemType.Treasure);
    }

    public void ShowMaterialsPanel()
    {
        GameService.Instance.StorageService.SetActivePanel(ItemType.Material);
    }

    public void ShowAllPanel()
    {
        GameService.Instance.StorageService.SetActivePanel(ItemType.None);
    }

    public void IncreaseQuantityOfItemSelected()
    {
        if (itemAmountSelected >= currentItemSelected.Quantity)
            return;

        int price = currentAction == CurrentAction.BUY ? currentItemSelected.BuyingPrice : currentItemSelected.SellingPrice;
        buySellInfoText.text = $"{++itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * price} coins";
    }
  
    public void DecreaseQuantityOfItemSelected()
    {
        if (itemAmountSelected <= 1)
            return;

        int price = currentAction == CurrentAction.BUY ? currentItemSelected.BuyingPrice : currentItemSelected.SellingPrice;
        buySellInfoText.text = $"{--itemAmountSelected} {currentItemSelected.name} FOR {itemAmountSelected * price} coins";
    }

    public void ShowPaymentConfirmation()
    {

        if (currentAction == CurrentAction.SELL)
        {
            DisplayTransactionStatusMessage($"SOLD {itemAmountSelected} {currentItemSelected.name} for {currentItemSelected.SellingPrice * itemAmountSelected}");
            GameService.Instance.EventService.OnSellTransactionInitiated.Invoke(currentItemSelected, itemAmountSelected);
        }

        else if(currentAction == CurrentAction.BUY)
        {
            DisplayTransactionStatusMessage($"BOUGHT {itemAmountSelected} {currentItemSelected.name} for {currentItemSelected.BuyingPrice * itemAmountSelected}");
            GameService.Instance.EventService.OnBuyTransactionInitiated.Invoke(currentItemSelected, itemAmountSelected);
        }

        CloseBuySellPanel();
        itemInfoPanel.SetActive(false);
    }

    private void DisplayTransactionStatusMessage(string message)
    {
        transactionStatusText.text = message;
        StartCoroutine(ClearTransactionStatusText());
    }
    private IEnumerator ClearTransactionStatusText()
    {
        if (transactionStatusTextAlreadyDisplayed)
            yield return null;

        transactionStatusTextAlreadyDisplayed = true;
        yield return new WaitForSecondsRealtime(transactionStatusTextDisplayTime);
        EmptyTransactionStatusText();
        transactionStatusTextAlreadyDisplayed = false;
    }

    private void EmptyTransactionStatusText() => transactionStatusText.text = string.Empty;
    private void CloseBuySellPanel()
    {
        buySellPanel.SetActive(false);
        itemAmountSelected = 0;
    }

    private void OpenBuySellPanel()
    {
        buySellPanel.SetActive(true);
        IncreaseQuantityOfItemSelected();
        buySellInfoHeading.text = $"HOW MANY DO YOU WANT TO {currentAction}?";
    }
}
