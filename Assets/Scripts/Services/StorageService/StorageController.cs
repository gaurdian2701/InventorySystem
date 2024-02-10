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
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private GameObject shopPanel;

    [Header("DATA LOAD PATHS")]
    [SerializeField] private string inventoryServiceDataLoadPath;
    [SerializeField] private string shopServiceDataLoadPath;

    [Header("INVENTORY SERVICE DATA")]
    [SerializeField] private InventoryScriptableObject inventoryScriptableObject;

    private List<RaycastResult> results;
    private GraphicRaycaster raycaster;

    public InventoryService inventoryService;
    public ShopService shopService;    

    protected override void Awake()
    {
        base.Awake();
        raycaster = GetComponent<GraphicRaycaster>();
        results = new List<RaycastResult>();

        inventoryService = new InventoryService(inventoryScriptableObject, inventoryPanel, inventoryServiceDataLoadPath);
        shopService = new ShopService(shopPanel, shopServiceDataLoadPath);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        raycaster.Raycast(eventData, results);

        HandleClickLogic();
        results.Clear();
    }

    public GameObject GetShopPanel() {  return shopPanel; }
    public GameObject GetInventoryPanel() { return inventoryPanel; } 

    public void GatherResources() { inventoryService.FillInventory(); }

    private void HandleClickLogic()
    {
        int layer = results[0].gameObject.transform.parent.gameObject.layer;
        int itemIndex = results[0].gameObject.transform.GetSiblingIndex(); 

        EventService.Instance.onItemUIClickedEvent.InvokeEvent(layer, itemIndex);
    }
}
