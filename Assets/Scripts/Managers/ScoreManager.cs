using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScoreManager 
{
    public string MyString = "COOL";
    public GameObject MainObject;
    // Start is called before the first frame update
    public ScoreManager()
    {
        MainObject = GameObject.Find("ScorePanel");
        Debug.Log("Score manager created");
    }
}
