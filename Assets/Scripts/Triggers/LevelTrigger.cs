using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTrigger : MonoBehaviour
{
    #region inspector

    [SerializeField]
    UnityEvent _onTriggered = null;

    #endregion

    #region private members
    
    LevelExit _exit = null;

    Animator _animator = null;

    bool _triggered = false;

    #endregion

    #region public members
    #endregion

    #region private methods

    void Awake()
    {
        _exit = transform.root?.GetComponentInChildren<LevelExit>();
        if (_exit != null)
        {
            _exit.triggerCount++;
        }
        _animator = GetComponent<Animator>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!_triggered && other.tag == "Player")
        {
            _triggered = true;
            OnTriggered();
        }
    }

    void OnTriggered()
    {
        // Trigger animation.
        _animator?.SetTrigger("Pressed");
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
