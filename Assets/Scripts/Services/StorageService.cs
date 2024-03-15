using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageService/* : IPointerClickHandler*/
{
    //[Header("MAIN UI PANELS")]
    //[SerializeField] private GameObject mainInventoryPanel;
    //[SerializeField] private GameObject mainShopPanel;

    //[Header("Item Type Panels")]
    //[SerializeField] private GameObject weaponsPanel;
    //[SerializeField] private GameObject consumablesPanel;
    //[SerializeField] private GameObject treasuresPanel;
    //[SerializeField] private GameObject materialsPanel;

    //[Header("DATA LOAD PATHS")]
    //[SerializeField] private string inventoryServiceDataLoadPath;
    //[SerializeField] private string shopServiceDataLoadPath;

    //[Header("INVENTORY SERVICE DATA")]
    //[SerializeField] private InventoryScriptableObject inventoryScriptableObject;

    private List<RaycastResult> results;
    private GraphicRaycaster raycaster;
    private GameObject activePanel;
    private List<GameObject> itemPanels;
    private InventoryService inventoryService;
    private ShopService shopService;
    private UIPanelData ui_Data;
    private InventoryScriptableObject inventoryScriptableObject;

    public StorageService(UIPanelData UIData, DataLoadPaths loadPathData, InventoryScriptableObject inventorySO)
    {
        ui_Data = UIData;
        inventoryScriptableObject = inventorySO;
        inventoryService = new InventoryService(inventorySO, UIData.InventoryPanel, loadPathData.InventoryServiceDataLoadPath);
        shopService = new ShopService(UIData, loadPathData.ShopServiceDataLoadPath);

        InitializePanels();

        GameService.Instance.EventService.OnBuyTransactionInitiated += DoBuyTransaction;
        GameService.Instance.EventService.OnSellTransactionInitiated += DoSellTransaction;
    }
    //protected override void Awake()
    //{
    //    raycaster = GetComponent<GraphicRaycaster>();
    //    results = new List<RaycastResult>();

    //    inventoryService = new InventoryService(inventoryScriptableObject, mainInventoryPanel, inventoryServiceDataLoadPath);
    //    shopService = new ShopService(mainShopPanel,
    //        weaponsPanel,
    //        consumablesPanel,
    //        treasuresPanel,
    //        materialsPanel,
    //        shopServiceDataLoadPath);

    //    InitializePanels();

    //    GameService.Instance.EventService.OnBuyTransactionInitiated += DoBuyTransaction;
    //    GameService.Instance.EventService.OnSellTransactionInitiated += DoSellTransaction;
    //}

    ~StorageService()
    {
        GameService.Instance.EventService.OnBuyTransactionInitiated -= DoBuyTransaction;
        GameService.Instance.EventService.OnSellTransactionInitiated -= DoSellTransaction;
    }
    private void InitializePanels()
    {
        activePanel = ui_Data.MainShopPanel;

        itemPanels = new List<GameObject>
        {
            ui_Data.MainShopPanel,
            ui_Data.WeaponsPanel,
            ui_Data.ConsumablesPanel,
            ui_Data.TreasuresPanel,
            ui_Data.MaterialsPanel
        };    

        SetActivePanel(ItemType.None);
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

    public InventoryService GetInventoryService() { return inventoryService;  }

    public ShopService GetShopService() { return shopService;}
    public void SetActivePanel(ItemType type) //Used to switch between different active panels
    {
        GameObject panelToBeActivated;

        switch (type)
        {
            case ItemType.Weapon:
                panelToBeActivated = ui_Data.WeaponsPanel; break;

            case ItemType.Consumable:
                panelToBeActivated = ui_Data.ConsumablesPanel; break;

            case ItemType.Treasure: 
                panelToBeActivated = ui_Data.TreasuresPanel; break;

            case ItemType.Material:
                panelToBeActivated = ui_Data.MaterialsPanel; break;

            default:
                panelToBeActivated = ui_Data.MainShopPanel; break;
        }

        panelToBeActivated.SetActive(true);
        shopService.SetCurrentList(type);

        foreach(var panel in itemPanels)
            if(panel != panelToBeActivated)
                panel.SetActive(false);
    }

    //public void OnPointerClick(PointerEventData eventData)
    //{
    //    raycaster.Raycast(eventData, results);

    //    HandleClickLogic();
    //    results.Clear();
    //}

    public GameObject GetActivePanel() {  return activePanel; }
    public GameObject GetInventoryPanel() { return ui_Data.InventoryPanel; } 

    public float GetInventoryWeightLimit() { return inventoryScriptableObject.WeightLimit; }
    public void GatherResources() { inventoryService.FillInventory(); }

    //private void HandleClickLogic()
    //{
    //    int layer = results[0].gameObject.transform.parent.gameObject.layer;
    //    int itemIndex = results[0].gameObject.transform.GetSiblingIndex(); //Getting the selected item

    //    GameService.Instance.EventService.OnItemUIClickedEvent.Invoke(layer, itemIndex);
    //}
}
