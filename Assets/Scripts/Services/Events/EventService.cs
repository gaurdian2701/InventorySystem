using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : GenericMonoSingleton<EventService>
{
    public EventController<int, int> onItemUIClickedEvent;
    protected override void Awake()
    {
        base.Awake();
    }

    public EventService()
    {
        onItemUIClickedEvent = new EventController<int, int>();
    }
}
