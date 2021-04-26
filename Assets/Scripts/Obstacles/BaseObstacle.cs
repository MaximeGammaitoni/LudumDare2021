using UnityEngine;
public abstract class BaseObstacle : MonoBehaviour
{
    #region private members

    bool _isInitialized = false;

    #endregion

    #region public members

    public int levelIndex { get; private set; }

    public bool isInitialized
    {
        get => _isInitialized;
        private set 
        {
            bool oldValue = _isInitialized;
            _isInitialized = value;
            if (!oldValue && _isInitialized)
            {
                OnInitialized();
            }
            else if (oldValue && !_isInitialized)
            {
                OnReset();
            }
        }
    }

    #endregion

    #region private methods

    protected virtual void Awake()
    {
        if (LevelLoader.isInit)
        {
            isInitialized = true;
            levelIndex = LevelLoader.currentLevelIndex;
        }
        else
        {
            levelIndex = LevelLoader.currentLevelIndex + 1;
            EventsManager.StartListening(nameof(StatesEvents.OnLandingIn), OnPlayerLanding);
        }
    }

    protected virtual void OnDestroy()
    {
        EventsManager.StopListening(nameof(StatesEvents.OnLandingIn), OnPlayerLanding);
    }

    void OnPlayerLanding(Args args)
    {
        if (levelIndex == LevelLoader.currentLevelIndex)
        {
            isInitialized = true;
        }
    }

    protected virtual void OnInitialized() {}

    protected virtual void OnReset() {}

    #endregion
}