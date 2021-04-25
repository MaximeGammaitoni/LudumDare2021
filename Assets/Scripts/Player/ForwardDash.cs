using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardDash : MonoBehaviour, IDashResponse
{
    public float _SpeedDash = 3f;
    public float _DashTime = 1f;
    private float _dashTime;

    private void Awake()
    {
        _dashTime = _DashTime;
    }
    public IEnumerator Dash(Vector2 playerMovement)
    {
        while (_dashTime > 0)
        {
            transform.position += _SpeedDash * transform.forward * Time.fixedDeltaTime;
            _dashTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _dashTime = _DashTime;
        yield return null;
    }
}
