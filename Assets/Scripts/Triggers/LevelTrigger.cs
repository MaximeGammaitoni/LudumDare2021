using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTrigger : MonoBehaviour
{
    #region inspector
    [SerializeField]
    private GameObject _particleSystem;
    [SerializeField]
    UnityEvent _onTriggered = null;

    [SerializeField]
    UnityEvent _onReset = null;

    #endregion

    #region private members

    LevelExit _exit = null;

    bool _triggered = false;

    #endregion

    #region public members
    #endregion

    #region private methods

    void Awake()
    {
        _exit = transform.root?.GetComponentInChildren<LevelExit>();
        EventsManager.StartListening("OnPlayerHit", PlayerHit);
        if (_exit != null)
        {
            _exit.triggerCount++;
        }
    }

    private void OnDestroy()
    {
        EventsManager.StopListening("OnPlayerHit", PlayerHit);

    }

    void PlayerHit(Args args)
    {
        _particleSystem.SetActive(false);
        _particleSystem.GetComponent<AutoDisable>().Timer =0 ;
        _onReset?.Invoke();
        _triggered = false;
    }

    void OnCollisionEnter(Collision other)
    {
        if (!_triggered && other.transform.tag == "Player" && 
            PlayerMovement.player != null && PlayerMovement.player.isDashing)
        {
            _triggered = true;
            OnTriggered();
            GameManager.singleton.SfxManager.PlayBoom();
        }
    }

    void OnTriggerStay(Collider other)
    {
        Debug.Log("GO BRRR !!!");
        Rigidbody rb = other.attachedRigidbody;
        GameObject go = rb?.gameObject;
        if (!_triggered && go != null && go.tag == "Player" && 
            PlayerMovement.player != null && PlayerMovement.player.isDashing)
        {
            _triggered = true;
            OnTriggered();
            GameManager.singleton.SfxManager.PlayBoom();
        }
    }

    void OnTriggered()
    {
        // Trigger animation.
        _particleSystem.SetActive(true);
        _onTriggered?.Invoke();
        if (_exit != null)
        {
            _exit.triggerPushedCount++;
        }
    }

    #endregion

    #region public methods
    #endregion
}
