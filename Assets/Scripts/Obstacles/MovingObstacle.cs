using System.Collections;
using UnityEngine;

public class MovingObstacle : MonoBehaviour
{
    #region inspector

    [SerializeField]
    Transform[] _movingPoints = null;

    [SerializeField]
    float _segmentDuration = 2f;

    [SerializeField]
    bool _isLoop = true;

    #endregion

    #region private members

    Coroutine _movingCoroutine = null;

    int _direction = 1;

    int _currentIndex = 0;

    float time = 0f;

    #endregion

    #region public members

    #endregion

    #region private methods

    void OnEnable()
    {
        _movingCoroutine = StartCoroutine(MovingCoroutine());
    }

    void OnDisable()
    {
        if (_movingCoroutine != null)
        {
            StopCoroutine(_movingCoroutine);
        }
    }
    
    IEnumerator MovingCoroutine()
    {
        if (_movingPoints.Length < 2)
        {
            yield break;
        }

        Transform from, to;
        while (true)
        {
            int nextIndex = (_currentIndex + _direction) % _movingPoints.Length;
            from = _movingPoints[_currentIndex];
            to = _movingPoints[nextIndex];
            while (time < _segmentDuration)
            {
                float t = time / _segmentDuration;
                transform.position = Vector3.Lerp(from.position, to.position, t);
                transform.rotation = Quaternion.Lerp(from.rotation, to.rotation, t);
                yield return null;
                time += Time.deltaTime;
            }
            time = 0f;
            _currentIndex = nextIndex;
            if (!_isLoop && (_currentIndex == _movingPoints.Length - 1 || _currentIndex == 0))
            {
                _direction *= -1;
            }
        }
    }

    #endregion

    #region public methods
    #endregion
}