using States;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [HideInInspector] public static GameManager singleton;

    [HideInInspector] public Dictionary<string, IEnumerator> coroutines;
    [HideInInspector] public delegate void GameEventManager();
    [HideInInspector] public static event GameEventManager SystemOnInit;

    [HideInInspector] public static event GameEventManager ApplicationQuitHandler;
    [HideInInspector] public static event GameEventManager ApplicationPauseHandler;
    [HideInInspector] public static event GameEventManager ApplicationFocusHandler;

    [HideInInspector] public static event GameEventManager GameUpdateHandler;
    [HideInInspector] public static event GameEventManager GameFixedUpdateHandler;

    [HideInInspector] public static event GameEventManager DeafetUiHandler;

    // Declare all your service here
    [HideInInspector] public ResourcesLoaderManager ResourcesLoaderManager;
    [HideInInspector] public PlayerEvents PlayerEvents { get; set; }

    [HideInInspector] public TimerManager TimerManager { get; set; }
    [HideInInspector] public StatesManager StatesManager { get; set; }
    [HideInInspector] public StatesEvents StatesEvents { get; set; }
    [HideInInspector] public PauseManager PauseManager { get; set; }
    [HideInInspector] public DefeatUIManager DefeatUIManager { get; set; }
    [HideInInspector] public LeaderBoardManager LeaderBoardManager { get; set; }
    [HideInInspector] public LevelsManager LevelsManager { get; set; }

    public void Awake()
    {
        GameUpdateHandler = null;
        DeafetUiHandler = null;
        GameFixedUpdateHandler = null;
        singleton = this;
        StartGameManager();
    }
    private void StartGameManager()
    {
        try
        {
            ResourcesLoaderManager = transform.GetComponentInChildren<ResourcesLoaderManager>();
            ResourcesLoaderManager.Init();
            EventsManager.Init();

            StatesEvents = new StatesEvents();
            StatesManager = new StatesManager();

            StatesManager.ChangeCurrentState(new Begin());
            StatesManager.ChangeCurrentState(new Run());
            PlayerEvents = new PlayerEvents();
            TimerManager = new TimerManager();
            PauseManager = new PauseManager();
            DefeatUIManager = new DefeatUIManager();
            LevelsManager = new LevelsManager();
            LeaderBoardManager = new LeaderBoardManager();
            //test
            LeaderBoardManager.GetRequestAndInstantiateIntoCanvas();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    public void OnDefeat()
    {
        Debug.Log($"DeafetUiHandler == null : {DeafetUiHandler == null}");
        DeafetUiHandler?.Invoke();
    }
    public void OnDisable()
    {

        //TODO : disable other game event
    }

    public void InitUnitySystem()
    {
        if (SystemOnInit != null)
        {
            Debug.Log(" GAME MANAGER INIT UNITY SYSTEM");
            SystemOnInit();
        }
    }

    private void Update()
    {
        GameUpdateHandler?.Invoke();
    }

    private void OnMouseDown()
    {

    }

    private void FixedUpdate()
    {
        GameFixedUpdateHandler?.Invoke();
    }
    public void StartCouroutineInGameManager(IEnumerator routine, string routineName)
    {
        if (coroutines == null)
        {
            coroutines = new Dictionary<string, IEnumerator>();
        }
        if (coroutines != null && !coroutines.ContainsKey(routineName))
        {
            Coroutine co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
        else if (coroutines != null && coroutines.ContainsKey(routineName))
        {
            StopCouroutineInGameManager(routineName);
            Coroutine l_co = StartCoroutine(routine);
            coroutines.Add(routineName, routine);
        }
    }
    public void StartCouroutineInGameManager(IEnumerator routine)//Coroutine avec arret automatique du MonoBehavior
    {
        StartCoroutine(routine);
    }
    public void StopCouroutineInGameManager(string coroutineName)
    {
        if (coroutines.ContainsKey(coroutineName))
        {
            StopCoroutine(coroutines[coroutineName]);
            coroutines.Remove(coroutineName);
        }
    }

    void OnApplicationQuit()
    {
        ApplicationQuitHandler?.Invoke();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            ApplicationFocusHandler?.Invoke();
        }
        else
        {
            ApplicationPauseHandler?.Invoke();
        }
    }

    public void DestroyServices()
    {
        StopAllCoroutines();
        DestroyAllManagers();
        DestroyAllClients();
        DestroyAllListeners();
        coroutines = null;
        //System.Web.HttpRuntime.UnloadAppDomain();
    }

    private void DestroyAllManagers()
    {
        ResourcesLoaderManager = null;
        PlayerEvents = null;

        TimerManager = null;
        StatesManager = null;
        StatesEvents = null;
        PauseManager = null;
        DefeatUIManager = null;
        LeaderBoardManager = null;
        LevelsManager = null;
    }
    private void DestroyAllClients()
    {

    }

    private void DestroyAllListeners()
    {
        ApplicationQuitHandler = null;
        ApplicationPauseHandler = null;
        ApplicationFocusHandler = null;
        GameUpdateHandler = null;
        GameFixedUpdateHandler = null;
        DeafetUiHandler = null;
    }
    public GameObject InstantiateInGameManager(UnityEngine.Object original, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(original, position, rotation) as GameObject;
        return go;
    }
}
