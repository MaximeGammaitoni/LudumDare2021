using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/GameConfig", order = 1)]
public class GameConfig : ScriptableObject
{
    public int RemainingTime;
}
