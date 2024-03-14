using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService
{
    public Action<int, int> OnItemUIClickedEvent;
    public Action<int, float> OnInventoryUpdated;

    public Action<ItemAdditionFailureType> OnItemAdditionFailure;
    public Action<ItemScriptableObject, int> OnBuyTransactionInitiated;
    public Action<ItemScriptableObject, int> OnSellTransactionInitiated;
}
