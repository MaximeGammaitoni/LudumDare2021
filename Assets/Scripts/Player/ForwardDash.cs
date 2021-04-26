using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForwardDash : MonoBehaviour, IDashResponse
{
    public float _SpeedDash = 3f;
    public float _DashTime = 1f;
    public float offset = 0.1f;
    private float _dashTime;
    private float _dashDistance;
    private MovementChecker _movementChecker;
    private PlayerMovement _playerMovement;
    private PlayerAnimationHandling _animatorHandler;

    private void Awake()
    {
        _movementChecker = transform.GetComponentInChildren<MovementChecker>();
        if (_movementChecker == null)
        {
            Debug.LogError("Movement checker is missing");
        }
        _playerMovement = transform.GetComponent<PlayerMovement>();
        if( _playerMovement == null)
        {
            Debug.LogError("PlayerMovement is not found");
        }

        _animatorHandler = GetComponentInChildren<PlayerAnimationHandling>();
        if (_animatorHandler == null)
        {
            Debug.LogError("_animatorHandler is not found");
        }
    }
    public IEnumerator Dash(Vector2 playerMovement)
    {
        _dashDistance = _SpeedDash * _DashTime;
        _dashTime = _DashTime;

        float maxDistance = _dashDistance;
        RaycastHit hit;
        if (Physics.Raycast(_playerMovement.RaycastOrigin.position, transform.forward, out hit, _dashDistance, _playerMovement._WallLayerMask))
        {
            maxDistance = hit.distance - 0.1f;
        }
        else if (Physics.Raycast(_playerMovement.RaycastOrigin.position, transform.forward, out hit, _dashDistance))
        {
            if (hit.collider.CompareTag("Interrupteur"))
            {
                maxDistance = Vector3.Distance(_playerMovement.RaycastOrigin.position, hit.transform.position);
                hit.collider.GetComponentInChildren<MeshRenderer>().enabled = false;
            }
        }
        Vector3 initialPos = transform.position;
        _animatorHandler.HasDashed(false);
        _animatorHandler.IsDashing(true);
        // Debug.Log("isdashing");
        while (_dashTime > 0)
        {
            Vector3 nextPos = _SpeedDash * transform.forward * Time.fixedDeltaTime;
            if (Vector3.Distance(initialPos, transform.position + nextPos) > maxDistance)
            {
                break;
            }
            transform.position += _SpeedDash * transform.forward * Time.fixedDeltaTime;
            _dashTime -= Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }
        _dashTime = _DashTime;
        _playerMovement._isDashing = false;
        _animatorHandler.IsDashing(false);
        _animatorHandler.HasDashed(true);

        // Debug.Log("Stopped Dashing");

        //Debug.Log("is not Dashing " + _playerMovement._isDashing);

        yield return null;
    }
}
