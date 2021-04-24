using UnityEngine;

public class LevelExit : MonoBehaviour
{
    #region inspector
    #endregion

    #region private members

    Collider _collider = null;

    Animator _animator = null;

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

    void Awake()
    {
        _animator = GetComponent<Animator>();
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
    }
    
    void OnTriggerEnter(Collider other)
    {
        if (_opened && !_exited && other.tag == "Player")
        {
            _exited = true;
            Exit(other.gameObject);
        }
    }

    void Open()
    {
        if (_opened)
        {
            return;
        }
        _opened = true;
        _animator?.SetTrigger("Opened");
        _collider.enabled = true;
    }

    void Exit(GameObject player)
    {
        GameManager.singleton.ResourcesLoaderManager.LevelLoader.MoveToNextLevel();
    }

    #endregion

    #region public methods
    #endregion
}