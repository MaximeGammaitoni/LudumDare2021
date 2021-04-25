using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using States;

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

    Collider _collider = null;

    PlayerControls _playerControls = null;

    Vector2 _movementDirection = Vector2.zero;

    bool _falling = false;

    bool _landing = false;

    bool _isDashing = false;

    private IDashResponse DashReponse;

    #endregion

    #region public members

    #endregion

    #region private methods

    void Awake()
    {
        _collider = GetComponent<Collider>();
        _playerControls = new PlayerControls();
       _playerControls.Enable();
       _playerControls.Main.Movement.performed += OnAxesChanged;
       _playerControls.Main.Movement.canceled += OnAxesChanged;
       _playerControls.Main.Dash.performed += OnDash;

        DashReponse = GetComponent<IDashResponse>();
        if (DashReponse == null)
        {
            Debug.LogError("Dash Response missing in PlayerMovement");
        }
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
        // Suscribe to state events.
        EventsManager.StartListening(nameof(StatesManager.OnStateChanged), OnStateChanged);
    }

    void OnDisable()
    {
        _playerControls?.Disable();
        // unsuscribe to state events.
        EventsManager.StopListening(nameof(StatesManager.OnStateChanged), OnStateChanged);
    }

    void OnStateChanged(Args args)
    {
        if (args is StateChangedArgs stateArgs && stateArgs.newState is Falling)
        {
            StartCoroutine(FallThroughGroundCoroutine());
        }
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
        if (DashReponse != null)
        {
            StartCoroutine(DashCoroutine());
        }
        else
        {
            _isDashing = false;
        }

    }

    void OnCollisionEnter(Collision other)
    {
        if (_falling && Mathf.Approximately(_collider.attachedRigidbody.velocity.y, 0f))
        {
            _landing = true;
        }
    }

    void FixedUpdate()
    {
        if (!GameManager.singleton.StatesManager.CurrentState.ElementsCanMove)
        {
            return;
        }
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

    IEnumerator FallThroughGroundCoroutine()
    {
        if (_falling)
        {
            yield break;
        }
        _falling = true;
        _landing = false;
        float playerHeight = _collider.bounds.size.y;
        float targetHeight = transform.position.y - playerHeight - Mathf.Epsilon;
        Rigidbody rb = _collider.attachedRigidbody;
        bool oldGravityState = rb.useGravity;
        // Make sure that gravity is enabled.
        rb.useGravity = true;
        _collider.enabled = false;
        // With that we make sure that the player has fallen through the ground.
        yield return new WaitUntil(() => transform.position.y <= targetHeight);
        _collider.enabled = true;
        // Wait for the player to fall on the ground.
        yield return new WaitUntil(() => _landing);
        Debug.Log("LANDING");
        _falling = false;
        _landing = false;
        rb.useGravity = oldGravityState;
        GameManager.singleton.StatesManager.CurrentState = new Landing();
        // Immediately regain movement (?)
        GameManager.singleton.StatesManager.CurrentState = new Run();
    }



    IEnumerator DashCoroutine()
    {
        yield return DashReponse.Dash(_movementDirection);
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