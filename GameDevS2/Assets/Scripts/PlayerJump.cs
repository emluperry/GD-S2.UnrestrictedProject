using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : MonoBehaviour
{
    //input
    private PlayerInput _input;

    //jump input
    private InputAction _jumpInputAction;
    private bool _isJumpDown;
    private float _jumpHeldDuration = 0;

    private Coroutine _jumpingCoroutine;
    private bool _isGrounded = true;

    //components
    private Rigidbody _rb;

    [Header("Jumping")]
    [SerializeField][Min(0f)] private float _minJumpForce = 3500f; //min height: 3.5? max height: 5?
    [SerializeField][Min(0f)] private float _extraJumpForce = 50f;
    [SerializeField][Min(0f)] private float _maxButtonHoldDuration = 1f;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _jumpInputAction = _input.currentActionMap.FindAction("Jump");

        _jumpInputAction.performed += Input_JumpPerformed;
        _jumpInputAction.canceled += Input_JumpCancelled;

        _rb = GetComponent<Rigidbody>();
    }

    #region INPUTS

    private void Input_JumpPerformed(InputAction.CallbackContext ctx)
    {
        _isJumpDown = ctx.ReadValueAsButton();

        if (_isJumpDown && _isGrounded && _jumpingCoroutine == null)
        {
            _jumpingCoroutine = StartCoroutine(c_JumpingCoroutine());
        }
    }

    private void Input_JumpCancelled(InputAction.CallbackContext ctx)
    {
        _isJumpDown = false;
        _jumpHeldDuration = 0;
    }

    #endregion

    private void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1))
        {
            _isGrounded = true;
        }
        else
        {
            _isGrounded = false;
        }
    }

    private IEnumerator c_JumpingCoroutine()
    {
        _isGrounded = false;

        yield return new WaitForFixedUpdate();

        _rb.AddForce(Vector3.up * _minJumpForce);

        float additionalForce = _extraJumpForce * (Time.fixedDeltaTime / _maxButtonHoldDuration);

        while (_isJumpDown && _jumpHeldDuration < _maxButtonHoldDuration)
        {
            yield return new WaitForFixedUpdate();

            _jumpHeldDuration += Time.fixedDeltaTime;
            _jumpHeldDuration = Mathf.Clamp(_jumpHeldDuration, 0, _maxButtonHoldDuration);

            _rb.AddForce(Vector3.up * additionalForce - (Physics.gravity * Time.fixedDeltaTime));
        }

        _jumpingCoroutine = null;
    }
}
