using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : GenericMonoSingleton<EventService>
{
    public EventController<int, int> onItemUIClickedEvent;
    public EventController<int, float> onInventoryUpdated;

    public EventController<ItemAdditionFailureType> onItemAdditionFailure;
    public EventController<ItemScriptableObject, int> onBuyTransactionInitiated;
    public EventController<ItemScriptableObject, int> onSellTransactionInitiated;

    protected override void Awake()
    {
        base.Awake();
    }

    public EventService()
    {
        onItemUIClickedEvent = new EventController<int, int>();
        onInventoryUpdated = new EventController<int, float>();
        onItemAdditionFailure = new EventController<ItemAdditionFailureType>();
        onBuyTransactionInitiated = new EventController<ItemScriptableObject, int>();
        onSellTransactionInitiated = new EventController<ItemScriptableObject, int>();
    }
}
