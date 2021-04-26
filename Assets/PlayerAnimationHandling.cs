using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationHandling : MonoBehaviour
{
    public Animator animator { get; set; }
    private bool isDead = false;
    private bool isExiting = false;
    private bool isDashing = false;
    private bool isMoving = false;
    [HideInInspector]
    public float speedMotion;
    private float lastSpeedMotion;

    public float deathTime { get; set; }

    private void Start()
    {
        animator = GetComponentInChildren<Animator>();
        EventsManager.StartListening("OnPlayerHit", IsHit);
        EventsManager.StartListening(nameof(StatesEvents.OnFallingIn), IsExit);
        EventsManager.StartListening(nameof(StatesEvents.OnFallingOut), IsStarting);
    }

    private void Update()
    { 

        if (!isDashing)
        {
            animator.SetFloat("Blend", speedMotion, 0.2f, Time.deltaTime);
        }
    }

    public void IsMoving()
    {
        isMoving = true;
    }

    public void IsDashing(bool value)
    {
        animator.SetBool("IsDashing", value);
        //lastSpeedMotion = speedMotion;
        //if (value)
        //{
        //    animator.SetFloat("Blend", 0);
        //}
        //else
        //{
        //    animator.SetFloat("Blend", lastSpeedMotion);
        //}

    }

    public void IsClosed()
    {
        isMoving = false;
    }

    public void HasDashed(bool value)
    {
        animator.SetBool("HasDashed", value);
    }

    public void IsHit(Args args)
    {
        animator.SetBool("IsDead", true);
        StartCoroutine(WaitForRezCoroutine());
    }

    public void IsExit(Args args)
    {
        Debug.Log("IsExit");
        animator.SetBool("IsExiting", true);
        Debug.Log("IsExit is " + animator.GetBool("IsExiting"));
    }

    public void IsStarting(Args args)
    {
        animator.SetBool("IsExiting", false);
    }

    IEnumerator WaitForRezCoroutine()
    {
        yield return new WaitForSeconds(deathTime);
        animator.SetBool("IsDead", false);
    }
}

