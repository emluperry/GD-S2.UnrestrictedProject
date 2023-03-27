using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerJump : EntityJump, IInput
{
    //jump input
    private InputAction _jumpInputAction;
    private bool _isJumpDown;
    private float _jumpHeldDuration = 0;

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _jumpInputAction = inputs["Jump"];

        _jumpInputAction.performed += Input_JumpPerformed;
        _jumpInputAction.canceled += Input_JumpCancelled;
    }

    protected void OnDestroy()
    {
        if (_jumpInputAction != null)
        {
            _jumpInputAction.performed -= Input_JumpPerformed;
            _jumpInputAction.canceled -= Input_JumpCancelled;
        }
    }

    #region INPUTS

    private void Input_JumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        _isJumpDown = ctx.ReadValueAsButton();

        if (_isJumpDown && _isGrounded && _jumpingCoroutine == null)
        {
            _jumpingCoroutine = StartCoroutine(c_JumpingCoroutine());
        }
    }

    private void Input_JumpCancelled(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        _isJumpDown = false;
        _jumpHeldDuration = 0;
    }

    #endregion

    protected override IEnumerator c_JumpingCoroutine()
    {
        yield return new WaitForFixedUpdate();

        StartJump();

        float additionalForce = _extraJumpForce * (Time.fixedDeltaTime / _maxButtonHoldDuration);

        while (_isJumpDown && _jumpHeldDuration < _maxButtonHoldDuration)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            _jumpHeldDuration += Time.fixedDeltaTime;
            _jumpHeldDuration = Mathf.Clamp(_jumpHeldDuration, 0, _maxButtonHoldDuration);

            _rb.AddForce(Vector3.up * additionalForce - (Physics.gravity * Time.fixedDeltaTime));
        }

        _jumpingCoroutine = null;
    }
}
