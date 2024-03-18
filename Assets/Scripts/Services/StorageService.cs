using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class StorageService
{
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

    public GameObject GetActivePanel() {  return activePanel; }
    public GameObject GetInventoryPanel() { return ui_Data.InventoryPanel; } 

    public float GetInventoryWeightLimit() { return inventoryScriptableObject.WeightLimit; }
    public void GatherResources() { inventoryService.FillInventory(); }
}
