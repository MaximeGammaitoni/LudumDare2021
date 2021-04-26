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

    [SerializeField]
    UnityEvent _onClosed = null;

    #endregion

    #region private members

    int _triggerPushedCount = 0;

    bool _opened = false;

    bool _exited = false;

    AnusDoorBehaviour doorBehaviour;
    
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
    private void OnDestroy()
    {
        EventsManager.StopListening("OnPlayerHit", PlayerHit);
    }

    private void Awake()
    {
        EventsManager.StartListening("OnPlayerHit", PlayerHit);
    }

    void PlayerHit(Args args)
    {
        _opened = false;
        _exited = false;
        _triggerPushedCount = 0;
        _onClosed?.Invoke();
        doorBehaviour._isOpened = false;

    }

    private void OnEnable()
    {
        doorBehaviour = GetComponentInChildren<AnusDoorBehaviour>();
    }

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
        doorBehaviour._isOpened = true;
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