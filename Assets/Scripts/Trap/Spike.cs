using System.Collections;
using MyBox;
using UnityEngine;
using UnityEngine.Events;

public class Spike : MonoBehaviour
{
    // Public declaration
    public bool NoPause = false;
    [ConditionalField(nameof(NoPause), true)]  public float TimeToTrigger = 2f;
    public UnityEvent OPD;
    

    // Private declaration
    private Animator TrapAnimator;
    private float _TimeRemaining = 0;
    private Collider _TrapCollider;


    private void Start()
    {
        TrapAnimator = this.GetComponent<Animator>();
        _TrapCollider = this.GetComponent<Collider>();
        if (NoPause)
            TimeToTrigger = 0;
        StartCoroutine(WaitBeforeTrigger());
    }


    private void OnTriggerEnter(Collider other)
    {

        Debug.Log("Box Collider");
        // TODO Wait More information before implement player death
        if (other.tag == "Player")
        {
            OPD?.Invoke();
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
                TrapAnimator.SetBool("NoPause", false);
                if (TrapAnimator.GetCurrentAnimatorStateInfo(0).IsName("Spike_trap") &&
                    TrapAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    TrapAnimator.SetBool("Play", false);
                    StartCoroutine(WaitBeforeTrigger());
                }
            }
            yield return null;

        }
       

    }
}

