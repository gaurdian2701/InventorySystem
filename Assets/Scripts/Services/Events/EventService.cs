using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : GenericMonoSingleton<EventService>
{
    public EventController<int, int> onItemUIClickedEvent;
    public EventController<int, float> onInventoryUpdated;
    protected override void Awake()
    {
        base.Awake();
    }

    public EventService()
    {
        onItemUIClickedEvent = new EventController<int, int>();
        onInventoryUpdated = new EventController<int, float>();
    }
}
