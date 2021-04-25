using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

using CallbackContext = UnityEngine.InputSystem.InputAction.CallbackContext;

public class PlayerMovement : MonoBehaviour
{
    #region inspector

    [SerializeField]
    float _speed = 2f;

    [SerializeField]
    bool _isDashable = true;

    #endregion

    #region private members

    PlayerControls _playerControls = null;

    Vector2 _movementDirection = Vector2.zero;

    bool _isDashing = false;

    private IDashResponse Dashresponse;

    #endregion

    #region public members

    #endregion

    #region private methods

    void Awake()
    {
       _playerControls = new PlayerControls();
       _playerControls.Enable();
       _playerControls.Main.Movement.performed += OnAxesChanged;
       _playerControls.Main.Movement.canceled += OnAxesChanged;
       _playerControls.Main.Dash.performed += OnDash;

        Dashresponse = GetComponent<IDashResponse>();
    }

    void OnDestroy()
    {
        _playerControls.Main.Movement.performed -= OnAxesChanged;
        _playerControls.Main.Movement.canceled -= OnAxesChanged;
        _playerControls.Main.Dash.performed -= OnDash;

    }

    void OnEnable()
    {
        _playerControls?.Enable();
    }

    void OnDisable()
    {
        _playerControls?.Disable();
    }

    void OnAxesChanged(CallbackContext ctx)
    {
        if (ctx.performed)
        {
            _movementDirection = ctx.ReadValue<Vector2>();
        }
        else if (ctx.canceled)
        {
            _movementDirection = Vector2.zero;
        }
    }

    void OnDash(CallbackContext ctx)
    {
        _isDashing = true;
        StartCoroutine(DashCoroutine());

    }

    void FixedUpdate()
    {
        if (!_isDashing)
        {
            Vector3 movement = Vector3.right * _movementDirection.x + Vector3.forward * _movementDirection.y;
            transform.position += _speed * movement * Time.fixedDeltaTime;
            if (movement != Vector3.zero)
            {
                transform.rotation = Quaternion.LookRotation(movement, Vector3.up);
            }
        }
       
    }

    IEnumerator DashCoroutine()
    {
        yield return Dashresponse.Dash(_movementDirection);
        _isDashing = false;
    }

    #endregion

    #region public methods

    #endregion
}

public interface IDashResponse
{
    IEnumerator Dash(Vector2 playerMovement);
}