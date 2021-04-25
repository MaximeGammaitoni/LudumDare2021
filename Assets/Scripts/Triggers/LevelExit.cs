using UnityEngine;
using UnityEngine.Events;

public class Lol
{
    public void Test(){}
}

public class LevelExit : MonoBehaviour
{
    #region inspector

    [SerializeField]
    UnityEvent _onOpened = null;

    #endregion

    #region private members

    int _triggerPushedCount = 0;

    bool _opened = false;

    bool _exited = false;
    
    #endregion

    #region public members

    public int triggerCount { get; set; } = 0;

    public int triggerPushedCount
    {
        get => _triggerPushedCount;
        set
        {
            _triggerPushedCount = value;
            if (_triggerPushedCount >= triggerCount)
            {
                Open();
            }
        }
    }

    #endregion

    #region private methods
    
    void OnTriggerEnter(Collider other)
    {
        if (_opened && !_exited && other.tag == "Player")
        {
            _exited = true;
            Exit();
        }
    }

    void Open()
    {
        if (_opened)
        {
            return;
        }
        _opened = true;
        _onOpened?.Invoke();
    }

    void Exit()
    {
        GameManager.singleton.StatesManager.CurrentState = new States.Falling();
        GameManager.singleton.ResourcesLoaderManager.LevelLoader.MoveToNextLevel();
    }

    #endregion

    #region public methods
    #endregion
}