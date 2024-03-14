using System;


public class EventController<T>
{
    public Action<T> BaseEvent;

    public void AddEventListener(Action<T> listener) => BaseEvent += listener;

    public void InvokeEvent(T type) => BaseEvent?.Invoke(type);

    public void RemoveEventListener(Action<T> listener) => BaseEvent -= listener;
}

public class EventController<T1, T2>
{
    public Action<T1, T2> BaseEvent;

    public void AddEventListener(Action<T1, T2> listener) => BaseEvent += listener;

    public void InvokeEvent(T1 type1, T2 type2) => BaseEvent?.Invoke(type1, type2);

    public void RemoveEventListener(Action<T1, T2> listener) => BaseEvent -= listener;
}