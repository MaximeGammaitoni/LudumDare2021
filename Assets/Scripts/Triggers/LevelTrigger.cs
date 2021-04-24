using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LevelTrigger : MonoBehaviour
{
    #region inspector

    [SerializeField]
    LevelExit _exit = null;

    [SerializeField]
    UnityEvent _onPressed = null;

    #endregion

    #region private members

    Animator _animator = null;

    bool _pressed = false;

    #endregion

    #region public members
    #endregion

    #region private methods

    void Awake()
    {
        if (_exit != null)
        {
            _exit.triggerCount++;
        }
        _animator = GetComponent<Animator>();
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (!_pressed && other.tag == "Player")
        {
            _pressed = true;
            OnPressed();
        }
    }

    void OnPressed()
    {
        // Trigger animation.
        _animator?.SetTrigger("Pressed");
        _onPressed?.Invoke();
        if (_exit != null)
        {
            _exit.triggerPushedCount++;
        }
    }

    #endregion

    #region public methods
    #endregion
}
