using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents
{
    public PlayerEvents()
    {
        PlayerDeathHandler += OnPlayerDeath;
        EventsManager.StartListening("OnPlayerDeath", PlayerDeathHandler);
        GameManager.singleton.StatesEvents.OnBeginIn += test;
    }

    public UnityAction<Args> PlayerDeathHandler;
    public class PlayerDeathArgs : Args
    {
        public GameObject PlayerGo;
    }
    private static void OnPlayerDeath(Args args)
    {
        if (args.GetType() != typeof(PlayerEvents.PlayerDeathArgs))
            throw new Exception("argument must be a PlayerDeathArgs");
        PlayerEvents.PlayerDeathArgs _args = ((PlayerEvents.PlayerDeathArgs)args);
        Debug.Log("lolilolk");

    }
    private static void Test(Args args)
    {
        if (args.GetType() != typeof(PlayerEvents.PlayerDeathArgs))
            throw new Exception("argument must be a PlayerDeathArgs");
        GameObject GO = ((PlayerDeathArgs)args).PlayerGo;
        GameManager.Instantiate(GO, Vector3.zero, Quaternion.identity);
    }
    public void PlayerIsDead()
    {
        EventsManager.TriggerEvent("OnPlayerDeath", new PlayerDeathArgs { PlayerGo = new GameObject("test") });
    }
    public void test(Args args)
    {
        Debug.Log("fsedfsdfs");
    }

}
