using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryService : GenericMonoSingleton<InventoryService>
{
    [SerializeField] private int itemLimit;
    [SerializeField] private float weightLimit;

    public List<ItemScriptableObject> inventoryItems {  get; private set; }
    
    protected override void Awake()
    {
        base.Awake();
        inventoryItems = new List<ItemScriptableObject>(itemLimit);
        var emptyItem = Resources.Load<ItemScriptableObject>("ItemSOs/EmptyItem");
        
        for(int i=0; i<inventoryItems.Capacity; i++)
            inventoryItems.Add(emptyItem);
    }
}
