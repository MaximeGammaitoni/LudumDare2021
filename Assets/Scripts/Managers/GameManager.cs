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


    // Declare all your service here
    [HideInInspector] public ResourcesLoaderManager ResourcesLoaderManager;
    [HideInInspector] public EventsManager EventsManager { get; set; }
    [HideInInspector] public PlayerEvents PlayerEvents { get; set; }
    
    [HideInInspector] public ScoreManager ScoreManager { get; set; }

    public void Awake()
    {
        GameUpdateHandler = null;
        GameFixedUpdateHandler = null;
        singleton = this;
        Debug.Log("singleton:" + singleton.ToString() + " is created");
        StartGameManager();
    }
    private void StartGameManager()
    {
        try
        {
            ResourcesLoaderManager = transform.GetComponentInChildren<ResourcesLoaderManager>();

            EventsManager = new EventsManager();
            ScoreManager = new ScoreManager();
            PlayerEvents = new PlayerEvents();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
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
        // define your services here
    }
    private void DestroyAllClients()
    {

    }

    private void DestroyAllListeners()
    {

    }
    public GameObject InstantiateInGameManager(UnityEngine.Object original, Vector3 position, Quaternion rotation)
    {
        var go = Instantiate(original, position, rotation) as GameObject;
        return go;
    }
}