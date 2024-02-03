using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShopService : GenericMonoSingleton<ShopService>
{
    private List<ItemScriptableObject> items;

    protected override void Awake()
    {
        var itemsList = Resources.LoadAll<ItemScriptableObject>("ItemSOs");
        items = new List<ItemScriptableObject>(itemsList);
    }
}
