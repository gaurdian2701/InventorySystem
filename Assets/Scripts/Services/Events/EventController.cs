using System;

public class EventController
{
    public Action baseEvent;

    public void AddEventListener(Action listener) => baseEvent += listener;

    public void InvokeEvent() => baseEvent?.Invoke();

    public void RemoveEventListener(Action listener) => baseEvent -= listener;
}

public class EventController<T1, T2>
{
    public Action<T1, T2> baseEvent;

    public void AddEventListener(Action<T1, T2> listener) => baseEvent += listener;

    public void InvokeEvent(T1 type1, T2 type2) => baseEvent?.Invoke(type1, type2);

    public void RemoveEventListener(Action<T1, T2> listener) => baseEvent -= listener;
}