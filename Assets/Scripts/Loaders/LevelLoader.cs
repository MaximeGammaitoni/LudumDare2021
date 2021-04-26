using System.Collections;
using UnityEngine;
using States;

public class LevelLoader : MonoBehaviour
{
    #region inspector

    [SerializeField]
    GameObject[] _levelPrefabs = null;

    [SerializeField]
    Transform _firstLevelOrigin = null;

    [SerializeField]
    float _heightBetweenLevels = 5f;

    [SerializeField]
    public Vector3 _playerOriginPosition = new Vector3(0, 0, 0);

    #endregion

    #region private members

    bool _mustDestroyPreviousLevel = false;

    GameObject _currentLevelInstance = null;

    GameObject _nextLevelInstance = null;
    GameObject _playerGo;

    #endregion

    #region public members

    public static int currentLevelIndex { get; private set; }

    public static bool isInit { get; private set; }

    #endregion

    #region private methods

    void OnEnable()
    {
        EventsManager.StartListening(nameof(StatesEvents.OnLandingIn), TriggerPreviousLevelDestruction);
        // Load 1st level.
        currentLevelIndex = 0;
        isInit = true;
        Vector3 position = _firstLevelOrigin?.position ?? Vector3.zero;
        _currentLevelInstance = Instantiate(_levelPrefabs[currentLevelIndex], position, Quaternion.identity);
        Debug.Log($"GameManager.singleton == null : {GameManager.singleton == null}");
        GameManager.singleton.LevelsManager.CurrentLevel = _currentLevelInstance;
        _playerGo = GameObject.FindGameObjectWithTag("Player");
        _playerGo.transform.position = _playerOriginPosition;
        isInit = false;
        LoadNextLevel();
    }

    void OnDestroy()
    {
        EventsManager.StopListening(nameof(StatesEvents.OnLandingIn), TriggerPreviousLevelDestruction);
    }

    void TriggerPreviousLevelDestruction(Args args)
    {
        _mustDestroyPreviousLevel = true;
    }

    void LoadNextLevel()
    {
        if (currentLevelIndex >= _levelPrefabs.Length - 1)
        {
            _nextLevelInstance = null;
            Debug.Log("No more level to load.");
            return;
        }
        
        Vector3 position = _currentLevelInstance.transform.position - Vector3.up * _heightBetweenLevels;
        _nextLevelInstance = Instantiate(_levelPrefabs[currentLevelIndex + 1], position, Quaternion.identity);
    }

    IEnumerator DestroyPreviousLevelCoroutine(GameObject previousLevel)
    {
        yield return new WaitUntil(() => _mustDestroyPreviousLevel); // À décommenter.
        _mustDestroyPreviousLevel = false;
        //yield return new WaitForSeconds(_timeBeforePreviousLevelDestroy); // À retirer.
        Destroy(previousLevel);
    }

    IEnumerator SetNewPLayerOriginPosition()
    {
        while (GameManager.singleton.StatesManager.CurrentState is States.Falling)
        {
            yield return null;
        }
        _playerOriginPosition = _playerGo.transform.position;
    }

    #endregion

    #region public methods
    
    public void MoveToNextLevel()
    {
        StartCoroutine(DestroyPreviousLevelCoroutine(_currentLevelInstance));
        StartCoroutine(SetNewPLayerOriginPosition());
        currentLevelIndex++;
        _currentLevelInstance = _nextLevelInstance;
        GameManager.singleton.LevelsManager.CurrentLevel = _currentLevelInstance;
        LoadNextLevel();
    }

    #endregion

}