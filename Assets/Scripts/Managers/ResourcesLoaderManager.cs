using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ResourcesLoaderManager : MonoBehaviour
{
    [Header("GameConfig")]
    public GameConfig GameConfig;

    [Header("ScirptableObjects")]
    public VFXLoader VFXLoader;
    public EnnemiesLoader EnnemiesLoader;

    [HideInInspector] public CanvasElements CanvasElements;
    public void Init()
    {
        CanvasElements = GetComponent<CanvasElements>();
    }
}