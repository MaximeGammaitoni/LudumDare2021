using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ProjectileThrower : MonoBehaviour
{
    #region inspector

    [SerializeField]
    GameObject _projectilePrefab = null;

    [SerializeField]
    Transform _spawnOrigin = null;

    [SerializeField]
    [Tooltip("The shoot cadency in seconds.")]
    float _cadency = 1f;

    [SerializeField]
    UnityEvent _onShoot = null;

    #endregion

    #region private members

    Coroutine _shootCoroutine = null;

    #endregion

    #region public members

    #endregion

    #region private methods

    void OnEnable()
    {
        _shootCoroutine = StartCoroutine(ShootProjectilesCoroutine());
    }

    void OnDisable()
    {
        if (_shootCoroutine != null)
        {
            StopCoroutine(_shootCoroutine);
        }
    }

    IEnumerator ShootProjectilesCoroutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(_cadency);
            _onShoot?.Invoke();
            Instantiate(_projectilePrefab, _spawnOrigin.position, _spawnOrigin.rotation);
        }
    }

    #endregion

    #region public methods

    #endregion
}
