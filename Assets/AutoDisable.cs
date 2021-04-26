using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AutoDisable : MonoBehaviour
{
    public float TimeToDisable;
    public float Timer = 0;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Timer += Time.deltaTime * Time.timeScale;
        if(Timer> TimeToDisable)
        {
            gameObject.SetActive(false);
        }

    }
}
