using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventService : GenericMonoSingleton<EventService>
{
    protected override void Awake()
    {
        base.Awake();
    }

    public EventController<int> onItemUIClicked { get; private set; }

    public EventService()
    {
        onItemUIClicked = new EventController<int>();
    }
}
