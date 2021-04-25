using System.Collections;
using UnityEngine;

public class LevelLoader : MonoBehaviour
{
    #region inspector

    [SerializeField]
    GameObject[] _levelPrefabs = null;

    [SerializeField]
    Transform _firstLevelOrigin = null;

    [SerializeField]
    float _heightBetweenLevels = 5f;

    #endregion

    #region private members

    bool _mustDestroyPreviousLevel = false;

    int _currentLevelIndex = 0;

    GameObject _currentLevelInstance = null;

    GameObject _nextLevelInstance = null;

    #endregion

    #region private methods

    void OnEnable()
    {
        EventsManager.StartListening(nameof(StatesEvents.OnLandingIn), TriggerPreviousLevelDestruction);
        // Load 1st level.
        _currentLevelIndex = 0;
        Vector3 position = _firstLevelOrigin?.position ?? Vector3.zero;
        _currentLevelInstance = Instantiate(_levelPrefabs[_currentLevelIndex], position, Quaternion.identity);
        LoadNextLevel();
    }

    void OnDisable()
    {
        EventsManager.StopListening(nameof(StatesEvents.OnLandingIn), TriggerPreviousLevelDestruction);
    }

    void TriggerPreviousLevelDestruction(Args args)
    {
        _mustDestroyPreviousLevel = true;
    }

    void LoadNextLevel()
    {
        if (_currentLevelIndex >= _levelPrefabs.Length - 1)
        {
            _nextLevelInstance = null;
            Debug.Log("No more level to load.");
            return;
        }
        
        Vector3 position = _currentLevelInstance.transform.position - Vector3.up * _heightBetweenLevels;
        _nextLevelInstance = Instantiate(_levelPrefabs[_currentLevelIndex + 1], position, Quaternion.identity);
    }

    IEnumerator DestroyPreviousLevelCoroutine(GameObject previousLevel)
    {
        yield return new WaitUntil(() => _mustDestroyPreviousLevel); // À décommenter.
        _mustDestroyPreviousLevel = false;
        //yield return new WaitForSeconds(_timeBeforePreviousLevelDestroy); // À retirer.
        Destroy(previousLevel);
    }

    #endregion

    #region public methods
    
    public void MoveToNextLevel()
    {
        StartCoroutine(DestroyPreviousLevelCoroutine(_currentLevelInstance));
        _currentLevelIndex++;
        _currentLevelInstance = _nextLevelInstance;
        LoadNextLevel();
    }

    #endregion

}