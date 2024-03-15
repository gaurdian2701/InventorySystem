using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransactionService
{
    private InventoryService inventoryService;
    private ShopService shopService;

    public TransactionService(InventoryService inventoryService, ShopService shopService)
    {
        this.inventoryService = inventoryService;
        this.shopService = shopService;

        GameService.Instance.EventService.OnBuyTransactionInitiated += DoBuyTransaction;
        GameService.Instance.EventService.OnSellTransactionInitiated += DoSellTransaction;
    }

    ~TransactionService() 
    {
        GameService.Instance.EventService.OnBuyTransactionInitiated -= DoBuyTransaction;
        GameService.Instance.EventService.OnSellTransactionInitiated -= DoSellTransaction;
    }

    private void DoBuyTransaction(ItemScriptableObject currentItemSelected, int itemAmountSelected)
    {
        if (!CheckBuyTransactionPossibility(currentItemSelected, itemAmountSelected))
            return;

        ItemScriptableObject itemToBeAdded = GameObject.Instantiate(currentItemSelected);
        itemToBeAdded.name = currentItemSelected.name;
        itemToBeAdded.Quantity = itemAmountSelected;

        inventoryService.AddItemToInventory(itemToBeAdded, itemAmountSelected);
        shopService.RemoveItemFromShop(currentItemSelected, itemAmountSelected);
    }

    //Function to check if the BUY transaction can be done or not.
    //If not, then it invokes an event that is listened to by the UI_InfoManager which in turn displays the appropriate failure message
    private bool CheckBuyTransactionPossibility(ItemScriptableObject itemToBeAdded, int itemAmountSelected)
    {
        if (!inventoryService.HasEnoughCoins(itemToBeAdded, itemAmountSelected))
        {
            GameService.Instance.EventService.OnItemAdditionFailure.Invoke(ItemAdditionFailureType.MONEY);
            return false;
        }

        else if (!inventoryService.HasEnoughWeight(itemToBeAdded, itemAmountSelected))
        {
            GameService.Instance.EventService.OnItemAdditionFailure.Invoke(ItemAdditionFailureType.WEIGHT);
            return false;
        }

        return true;
    }
    private void DoSellTransaction(ItemScriptableObject currentItemSelected, int itemAmountSelected)
    {
        ItemScriptableObject itemToBeAdded = GameObject.Instantiate(currentItemSelected);
        itemToBeAdded.name = currentItemSelected.name;
        itemToBeAdded.Quantity = itemAmountSelected;

        inventoryService.RemoveItemFromInventory(currentItemSelected, itemAmountSelected);
        shopService.AddItemToShop(itemToBeAdded, itemAmountSelected);
    }
}
