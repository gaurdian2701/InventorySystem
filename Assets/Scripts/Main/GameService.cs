using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameService : GenericMonoSingleton<GameService>
{
    public EventService EventService {  get; private set; }
    protected override void Awake()
    {
        base.Awake();

        EventService = new EventService();
    }


}
