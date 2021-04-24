using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Spike : MonoBehaviour
{
    // Public declaration
    public float TimeToTrigger = 2f;
    public bool NoPause = false;

    // Private declaration
    private Animator TrapAnimator;
    private float _TimeRemaining = 0;
    private Collider _TrapCollider;


    private void Start()
    {
        TrapAnimator = this.GetComponent<Animator>();
        _TrapCollider = this.GetComponent<Collider>();
        StartCoroutine(WaitBeforeTrigger());
    }


    private void OnTriggerEnter(Collider other)
    {
        // TODO Wait More information before implement player death
        if (other.tag == "Player")
        {
            GameManager.singleton.PlayerEvents.PlayerIsDead();
        }
    }

    IEnumerator WaitBeforeTrigger()
    {
        while (!TrapAnimator.GetBool("Play"))
        {
            if (TimeToTrigger > _TimeRemaining)
            {
                _TimeRemaining += 1;
            }
            else
            {
                _TimeRemaining = 0;
                TrapAnimator.SetBool("Play", true);
                StartCoroutine(Animation());
            }
            yield return new WaitForSeconds(1);

        }

    }

    IEnumerator Animation()
    {
        while (TrapAnimator.GetBool("Play")) {
            if (NoPause)
            {
                TrapAnimator.SetBool("NoPause", true);
                TrapAnimator.SetBool("EndAnimation", false);
                if (TrapAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spike_trap") &&
                    TrapAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    TrapAnimator.SetBool("EndAnimation", true);
                }
            }
            else
            {
                if (TrapAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spike_trap"))
                {
                    _TrapCollider.enabled = true;
                }
                if (TrapAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spike_trap") &&
                    TrapAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    _TrapCollider.enabled = false;
                    TrapAnimator.SetBool("Play", false);
                    StartCoroutine(WaitBeforeTrigger());
                }
            }
            yield return null;

        }
       

    }
}

