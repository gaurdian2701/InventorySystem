using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameService : GenericMonoSingleton<GameService>, IPointerClickHandler
{
    [Header("MAIN UI PANELS")]
    [SerializeField] private GameObject mainInventoryPanel;
    [SerializeField] private GameObject mainShopPanel;

    [Header("ITEM TYPE PANELS")]
    [SerializeField] private GameObject weaponsPanel;
    [SerializeField] private GameObject consumablesPanel;
    [SerializeField] private GameObject treasuresPanel;
    [SerializeField] private GameObject materialsPanel;

    [Header("DATA LOAD PATHS")]
    [SerializeField] private string inventoryServiceDataLoadPath;
    [SerializeField] private string shopServiceDataLoadPath;

    [Header("INVENTORY SERVICE DATA")]
    [SerializeField] private InventoryScriptableObject inventoryScriptableObject;

    [Header("INPUT")]
    [SerializeField] private GraphicRaycaster raycaster;
    private List<RaycastResult> results;
    public EventService EventService {  get; private set; }
    public StorageService StorageService { get; private set; }
    public TransactionService TransactionService { get; private set; }
    protected override void Awake()
    {
        base.Awake();

        results = new List<RaycastResult>();
        EventService = new EventService();
        InitializeStorageService();
        TransactionService = new TransactionService(StorageService.GetInventoryService(), StorageService.GetShopService());
    }

    private void InitializeStorageService()
    {

        UIPanelData UIData = new UIPanelData(mainShopPanel, mainInventoryPanel, weaponsPanel, consumablesPanel, treasuresPanel, materialsPanel);
        DataLoadPaths dataLoadPaths = new DataLoadPaths(inventoryServiceDataLoadPath, shopServiceDataLoadPath);
        StorageService = new StorageService(UIData, dataLoadPaths, inventoryScriptableObject);
    }

    private void HandleClickLogic()
    {
        int layer = results[0].gameObject.transform.parent.gameObject.layer;
        int itemIndex = results[0].gameObject.transform.GetSiblingIndex(); //Getting the selected item

        EventService.OnItemUIClickedEvent.Invoke(layer, itemIndex);
    }
    public void OnPointerClick(PointerEventData eventData)
    {
        raycaster.Raycast(eventData, results);

        HandleClickLogic();
        results.Clear();
    }

    public void GatherResources() => StorageService.GatherResources();
}

public struct UIPanelData
{
    public GameObject MainShopPanel {  get; private set; }
    public GameObject InventoryPanel {  get; private set; }
    public GameObject WeaponsPanel {  get; private set; }
    public GameObject ConsumablesPanel {  get; private set; }
    public GameObject TreasuresPanel {  get; private set; }
    public GameObject MaterialsPanel {  get; private set; }

    public UIPanelData(GameObject _MainShopPanel,
        GameObject _InventoryPanel,
        GameObject _WeaponsPanel,
        GameObject _ConsumablesPanel,
        GameObject _TreasuresPanel,
        GameObject _MaterialsPanel)
    {
        MainShopPanel = _MainShopPanel;
        InventoryPanel = _InventoryPanel;
        WeaponsPanel = _WeaponsPanel;
        ConsumablesPanel = _ConsumablesPanel;   
        TreasuresPanel = _TreasuresPanel;
        MaterialsPanel = _MaterialsPanel;
    }
}

public struct DataLoadPaths
{
    public string InventoryServiceDataLoadPath {  get; private set; }
    public string ShopServiceDataLoadPath { get; private set; }

    public DataLoadPaths(string InventoryDataPath, string ShopDataPath)
    {
        InventoryServiceDataLoadPath = InventoryDataPath;
        ShopServiceDataLoadPath = ShopDataPath;
    }
}