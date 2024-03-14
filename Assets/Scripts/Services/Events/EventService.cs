using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : GenericMonoSingleton<EventService>
{
    public EventController<int, int> OnItemUIClickedEvent;
    public EventController<int, float> OnInventoryUpdated;

    public EventController<ItemAdditionFailureType> OnItemAdditionFailure;
    public EventController<ItemScriptableObject, int> OnBuyTransactionInitiated;
    public EventController<ItemScriptableObject, int> OnSellTransactionInitiated;

    protected override void Awake()
    {
        base.Awake();
    }

    public EventService()
    {
        OnItemUIClickedEvent = new EventController<int, int>();
        OnInventoryUpdated = new EventController<int, float>();
        OnItemAdditionFailure = new EventController<ItemAdditionFailureType>();
        OnBuyTransactionInitiated = new EventController<ItemScriptableObject, int>();
        OnSellTransactionInitiated = new EventController<ItemScriptableObject, int>();
    }
}
