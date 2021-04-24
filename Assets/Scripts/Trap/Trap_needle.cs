using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trap_needle : MonoBehaviour
{
    // Public declaration
    public float TimeToTrigger = 2f;
    public Transform Needle;
    public float TimeTriggered = 2f;
    public float NeedleFaster = 0.03f;

    // Private declaration
    private bool _CanBeTrigger = false;
    private float _TimeRemaining = 0;
    private Coroutine _InProgress = null;
    private Coroutine _Waiting = null;
    private bool _CanWait = true;
    //private PlayerEvents _PlayerEvents;
    // private PlayerEvents.PlayerDeathArgs _PlayerDeathArgs;

    // Update is called once per frame
    void Update()
    {

        if (_CanBeTrigger)
        {
            if (_InProgress == null)
            {

                _InProgress = StartCoroutine(Animation());
            }

        }
        else if (_Waiting == null)
        {
             _Waiting = StartCoroutine(WaitBeforeTrigger());
            
        }

    }


    private void OnTriggerEnter(Collider other)
    {
        // TODO Wait More information before implement player death
        //if (_PlayerDeathArgs.PlayerGo.GetComponent<Collider>() == other) {\
            //GameManager.singleton.PlayerEvents.PlayerIsDead();
        //}
    }

    IEnumerator WaitBeforeTrigger()
    {
        while (_CanWait)
        {
            if (TimeToTrigger > _TimeRemaining)
            {
                _TimeRemaining += 1;
            }
            else
            {
                _TimeRemaining = 0;
                _CanBeTrigger = true;
            }
            yield return new WaitForSeconds(1);
        }
        _CanWait = false;
        _Waiting = null;

    }


    IEnumerator Animation()
    {
        float new_y_position;
        Vector3 initial_position = Needle.transform.position;
        Vector3 destination_position = new Vector3(
            Needle.transform.position.x + Needle.transform.lossyScale.x,
            Needle.transform.position.y + Needle.transform.lossyScale.y,
            Needle.transform.position.z + Needle.transform.lossyScale.z);

        
        while (Needle.position.y < destination_position.y)
        {
            new_y_position = Needle.position.y + NeedleFaster;
            if (new_y_position > destination_position.y)
            {
                new_y_position = destination_position.y;
            }
            Needle.position = new Vector3(Needle.position.x, new_y_position, Needle.position.z);
            yield return null;

        }
        yield return new WaitForSeconds(TimeTriggered);

        while (Needle.position.y > initial_position.y)
        {
            new_y_position = Needle.position.y - NeedleFaster;
            if (new_y_position < initial_position.y)
            {
                new_y_position = initial_position.y;
            }
            Needle.position = new Vector3(Needle.position.x, new_y_position, Needle.position.z);
            yield return null;

        }
        _CanWait = true;
        _CanBeTrigger = false;
        _InProgress = null;
    }
}
