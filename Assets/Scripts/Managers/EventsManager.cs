using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public abstract class Args
{

}
public class UnityCustomEvents<T> : UnityEvent<T>
{

}
public class EventsManager
{

    private Dictionary<string, UnityCustomEvents<Args>> eventDictionary;

    public EventsManager()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityCustomEvents<Args>>();
        }
    }

    public void StartListening(string eventName, UnityAction<Args> listener)
    {
        UnityCustomEvents<Args> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.AddListener(listener);
        }
        else
        {
            thisEvent = new UnityCustomEvents<Args>();
            thisEvent.AddListener(listener);
            eventDictionary.Add(eventName, thisEvent);
        }
    }

    public void StopListening(string eventName, UnityAction<Args> listener)
    {
        if (this == null) return;
        UnityCustomEvents<Args> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public void TriggerEvent(string eventName, Args args)
    {
        UnityCustomEvents<Args> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(args);
        }
    }



}
