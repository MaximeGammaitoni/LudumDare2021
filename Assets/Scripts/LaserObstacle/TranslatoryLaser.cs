using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TranslatoryLaser : MonoBehaviour, ILaserType
{
    public Transform _StartObject;
    public Transform _TargetObject;
    public bool _isSpeedRandom;
    public float _TravelSpeed;
    public bool _IsPausingAtGoal;
    public float _PauseTime;

    private float _distance;
    private float _step = 0;
    private List<Transform> _listOrder = new List<Transform>();
    private IEnumerator _pauseCoroutine = null;
    // Start is called before the first frame update
    void Start()
    {
        if(_StartObject != null || _TargetObject != null)
        {
            _distance = Vector3.Distance(_StartObject.position, _TargetObject.position);
            this.transform.position = _StartObject.position;

            InitializeList();
        }
        else
        {
            Debug.LogWarning("StartObject or TargetObject is null on " + transform.gameObject.name);
        }
    }

    private void InitializeList()
    {
        _listOrder.Add(_StartObject);
        _listOrder.Add(_TargetObject);
    }

    // Update is called once per frame
    void Update()
    {
        if (_pauseCoroutine == null)
        {
            LaserMovement();
        }

        if (_step/_distance > 0.9999)
        {
            // Interverting transforms
            SwapObjects();

            // Checking if we want to pause at the end of travel
            if(_IsPausingAtGoal)
            {
                _pauseCoroutine = PauseCoroutine();
                StartCoroutine(_pauseCoroutine);
            }
        }

    }

    private void SwapObjects()
    {
        Transform temp = _listOrder[0];
        _listOrder[0] = _listOrder[1];
        _listOrder[1] = temp;
        _step = 0;
    }

    public void LaserMovement()
    {
        StepCalculation(_isSpeedRandom);
        transform.position = Vector3.Lerp(_listOrder[0].position, _listOrder[1].position, StepCalculation(_isSpeedRandom) / _distance);
    }

    private float StepCalculation(bool isRandom = false)
    {
        if(isRandom)
        {
            float speed = Random.Range(0, _TravelSpeed * 5);
            _step += Time.deltaTime * speed;
            return _step;
        }
        else
        {
            _step += Time.deltaTime * _TravelSpeed;
            return _step;
        }

    }


    // Pause time 
    public IEnumerator PauseCoroutine()
    {
        yield return new WaitForSecondsRealtime(_PauseTime);

        // Setting coroutine variable to null to know when to start moving again
        _pauseCoroutine = null;
    }

   
}
