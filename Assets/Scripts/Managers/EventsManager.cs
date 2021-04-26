using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;
using System;

public static class EventsManager
{
    private static Dictionary<string, UnityCustomEvents<Args>> eventDictionary = new Dictionary<string, UnityCustomEvents<Args>>();

    public static void Init()
    {
        if (eventDictionary == null)
        {
            eventDictionary = new Dictionary<string, UnityCustomEvents<Args>>();
        }
    }

    public static void StartListening(string eventName, UnityAction<Args> listener)
    {
        if (listener == null) return;
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
    public static void StopListeningAll()
    {
        eventDictionary = null;
    }

    public static void StopListening(string eventName, UnityAction<Args> listener)
    {

        UnityCustomEvents<Args> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.RemoveListener(listener);
        }
    }

    public static void TriggerEvent(string eventName, Args args = null)
    {
        if (args == null) args = new Args();
        UnityCustomEvents<Args> thisEvent = null;
        if (eventDictionary.TryGetValue(eventName, out thisEvent))
        {
            thisEvent.Invoke(args);
        }
    }
    public static void EmptyListener(Args args)
    {
        Debug.Log("you try to call an emptu listner");
    }
}
