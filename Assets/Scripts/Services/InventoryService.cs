using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryService : GenericMonoSingleton<InventoryService>, IPointerClickHandler
{
    [SerializeField] private GraphicRaycaster raycaster;
    [SerializeField] private GameObject inventoryPanel;
    [SerializeField] private int itemLimit;
    [SerializeField] private float weightLimit;

    private StorageUI inventoryUI;
    private List<RaycastResult> results;
    public List<ItemScriptableObject> inventoryItems {  get; private set; }

    protected override void Awake()
    {
        base.Awake();

        raycaster = GetComponent<GraphicRaycaster>();
        var emptyItem = Resources.LoadAll<ItemScriptableObject>("ItemSOs/EmptyItem");
        inventoryItems = new List<ItemScriptableObject>(itemLimit);
        results = new List<RaycastResult>();

        InitializeInventoryUI(emptyItem);
    }

    private void InitializeInventoryUI(ItemScriptableObject[] itemList)
    {
        var itemsUIList = new List<ItemScriptableObject>(itemList);
        inventoryUI = new StorageUI(inventoryPanel, itemsUIList);
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        raycaster.Raycast(eventData, results);
        EventService.Instance.onItemUIClicked.InvokeEvent(results[0].gameObject.transform.GetSiblingIndex());
        results.Clear();
    }
}
