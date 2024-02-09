using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopService : GenericMonoSingleton<ShopService>
{
    [SerializeField] private GameObject shopPanel;
    [SerializeField] private List<ItemScriptableObject> shopItems;
    
    private StorageUI shopUI;

    protected override void Awake()
    {
        base.Awake();
        var shopItemsList = Resources.LoadAll<ItemScriptableObject>("ItemSOs");

        InitializeShopUI(shopItemsList);
    }

    private void Start()
    {
        EventService.Instance.onItemUIClicked.AddEventListener(HandleUIClickedEvent);
    }

    private void HandleUIClickedEvent(int itemIndex)
    {
        shopPanel.transform.GetChild(itemIndex).gameObject.SetActive(false);
    }

    private void InitializeShopUI(ItemScriptableObject[] shopItemsList)
    {
        shopItems = new List<ItemScriptableObject>(shopItemsList);
        shopUI = new StorageUI(shopPanel, shopItems);
    }
}
