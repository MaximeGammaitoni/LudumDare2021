using System;
using System.Collections;
using System.Collections.Generic;
using States;
using UnityEngine;
using UnityEngine.Events;

public class PlayerEvents
{
    public PlayerEvents()
    {
        PlayerDeathHandler += OnPlayerDeath;
        EventsManager.StartListening("OnPlayerDeath", PlayerDeathHandler);

        PlayerHitHandler += OnPlayerHit;
        EventsManager.StartListening("OnPlayerHit", PlayerHitHandler);


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
    }
    public void PlayerIsDead()
    {
        Debug.Log("Player is dead");
        // not False  or not False
        if (!(GameManager.singleton.StatesManager.CurrentState is End) &&
            !(GameManager.singleton.StatesManager.CurrentState is Win) &&
            !(GameManager.singleton.StatesManager.CurrentState is Pause) &&
            !(GameManager.singleton.StatesManager.CurrentState is Landing) &&
            !(GameManager.singleton.StatesManager.CurrentState is Falling))
        {
            Debug.Log("Ta mere la chauve.");
            EventsManager.TriggerEvent("OnPlayerDeath", new PlayerDeathArgs());
            GameManager.singleton.StatesManager.CurrentState = new States.End();
            GameManager.singleton.OnDefeat();
        }

    }

    public UnityAction<Args> PlayerHitHandler;
    private static void OnPlayerHit(Args args)
    {

    }

    public void PlayerHit()
    {
        //GameManager.singleton.TimerManager.RemoveTime();
        PlayerMovement.player.transform.position = GameManager.singleton.ResourcesLoaderManager.LevelLoader._playerOriginPosition;
        EventsManager.TriggerEvent("OnPlayerHit", new PlayerDeathArgs());
        GameManager.singleton.SfxManager.Playhit();
    }

    public void test(Args args)
    {
        Debug.Log("fsedfsdfs");
    }

}
