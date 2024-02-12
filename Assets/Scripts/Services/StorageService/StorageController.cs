using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageController : GenericMonoSingleton<StorageController>, IPointerClickHandler
{
    [Header("MAIN UI PANELS")]
    [SerializeField] private GameObject mainInventoryPanel;
    [SerializeField] private GameObject mainShopPanel;

    [Header("Item Type Panels")]
    [SerializeField] private GameObject weaponsPanel;
    [SerializeField] private GameObject consumablesPanel;
    [SerializeField] private GameObject treasuresPanel;
    [SerializeField] private GameObject materialsPanel;

    [Header("DATA LOAD PATHS")]
    [SerializeField] private string inventoryServiceDataLoadPath;
    [SerializeField] private string shopServiceDataLoadPath;

    [Header("INVENTORY SERVICE DATA")]
    [SerializeField] private InventoryScriptableObject inventoryScriptableObject;

    private List<RaycastResult> results;
    private GraphicRaycaster raycaster;
    private GameObject activePanel;
    private List<GameObject> itemPanels;

    private InventoryService inventoryService;
    private ShopService shopService;    

    protected override void Awake()
    {
        base.Awake();
      
        raycaster = GetComponent<GraphicRaycaster>();
        results = new List<RaycastResult>();

        inventoryService = new InventoryService(inventoryScriptableObject, mainInventoryPanel, inventoryServiceDataLoadPath);
        shopService = new ShopService(mainShopPanel,
            weaponsPanel,
            consumablesPanel,
            treasuresPanel,
            materialsPanel,
            shopServiceDataLoadPath);

        InitializePanels();
    }

    private void InitializePanels()
    {
        activePanel = mainShopPanel;

        itemPanels = new List<GameObject>
        {
            mainShopPanel,
            weaponsPanel,
            consumablesPanel,
            treasuresPanel,
            materialsPanel
        };    

        SetActivePanel(ItemType.None);
    }

    public void DoBuyTransaction(ItemScriptableObject currentItemSelected, int itemAmountSelected)
    {
        ItemScriptableObject itemToBeAdded = GameObject.Instantiate(currentItemSelected);
        itemToBeAdded.name = currentItemSelected.name;
        itemToBeAdded.quantity = itemAmountSelected;

        shopService.RemoveItemFromShop(currentItemSelected, itemAmountSelected);
        inventoryService.AddItemToInventory(itemToBeAdded, itemAmountSelected);
    }

    public void DoSellTransaction(ItemScriptableObject currentItemSelected, int itemAmountSelected)
    {
        ItemScriptableObject itemToBeAdded = GameObject.Instantiate(currentItemSelected);
        itemToBeAdded.name = currentItemSelected.name;
        itemToBeAdded.quantity = itemAmountSelected;

        inventoryService.RemoveItemFromInventory(currentItemSelected, itemAmountSelected);
        shopService.AddItemToShop(itemToBeAdded, itemAmountSelected);
    }

    public InventoryService GetInventoryService() { return inventoryService;  }

    public ShopService GetShopService() { return shopService;}
    public void SetActivePanel(ItemType type)
    {
        GameObject panelToBeActivated;

        switch (type)
        {
            case ItemType.Weapon:
                panelToBeActivated = weaponsPanel; break;

            case ItemType.Consumable:
                panelToBeActivated = consumablesPanel; break;

            case ItemType.Treasure: 
                panelToBeActivated = treasuresPanel; break;

            case ItemType.Material:
                panelToBeActivated = materialsPanel; break;

            default:
                panelToBeActivated = mainShopPanel; break;
        }

        panelToBeActivated.SetActive(true);
        shopService.SetCurrentList(type);

        foreach(var panel in itemPanels)
            if(panel != panelToBeActivated)
                panel.SetActive(false);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        raycaster.Raycast(eventData, results);

        HandleClickLogic();
        results.Clear();
    }

    public GameObject GetActivePanel() {  return activePanel; }
    public GameObject GetInventoryPanel() { return mainInventoryPanel; } 

    public void GatherResources() { inventoryService.FillInventory(); }

    private void HandleClickLogic()
    {
        int layer = results[0].gameObject.transform.parent.gameObject.layer;

        foreach(var ui in results)
            Debug.Log(ui.gameObject.name);

        int itemIndex = results[0].gameObject.transform.GetSiblingIndex(); 

        EventService.Instance.onItemUIClickedEvent.InvokeEvent(layer, itemIndex);
    }
}
