using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ResourcesLoaderManager : MonoBehaviour
{
    [Header("GameConfig")]
    public GameConfig GameConfig;

    [Header("DataTemplate")]
    public GameObject LeaderBoardLine;
    public GameObject LeaderBoardTable;

    [Header("ScriptableObjects")]
    public VFXLoader VFXLoader;
    public EnnemiesLoader EnnemiesLoader;
    public UIElementsLoader UIElementsLoader;

    [Header("SFX")]
    public AudioSource MainSFX;
    public AudioClip Dash;
    public AudioClip Boom;
    public AudioClip Win;
    public AudioClip Lose;
    public AudioClip ExitOpen;
    public AudioClip PlayerHit;


    [HideInInspector] public CanvasElements CanvasElements;
    [HideInInspector] public LevelLoader LevelLoader;

    public void Init()
    {
        CanvasElements = GetComponent<CanvasElements>();
        LevelLoader = GetComponent<LevelLoader>();
    }
}