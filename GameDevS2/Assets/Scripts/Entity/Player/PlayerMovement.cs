using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : EntityMovement, IInput
{
    //movement input
    private InputAction _moveInputAction;

    [Header("External References")]
    [SerializeField] private Transform _cameraTransform;

    #region INPUT SETUP
    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _moveInputAction = inputs["Move"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_moveInputAction != null)
        {
            _moveInputAction.performed += Input_MovePerformed;
            _moveInputAction.canceled += Input_MoveCancelled;
        }
    }

    public void DisableInput()
    {
        if (_moveInputAction != null)
        {
            _moveInputAction.performed -= Input_MovePerformed;
            _moveInputAction.canceled -= Input_MoveCancelled;
        }
    }
    #endregion

    protected void OnDestroy()
    {
        DisableInput();
    }

    #region INPUTS

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        Vector2 input = ctx.ReadValue<Vector2>();
        _moveInput = new Vector3(input.x, 0, input.y);

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }

        if (_moveInput.sqrMagnitude > 0 && _movementCoroutine == null)
        {
            _isMoving = true;
            _movementCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        _moveInput = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _isMoving = false;

        if(_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _movementCoroutine = StartCoroutine(c_StoppingCoroutine());
    }

    #endregion

    protected override IEnumerator c_MovementCoroutine()
    {
        while (_isMoving)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            //update input direction
            _moveDirection = _cameraTransform.TransformDirection(_moveInput);
            _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);

            MoveEntity();
            RotateEntity();
        }
    }

    public Transform GetCameraTransform()
    {
        return _cameraTransform;
    }
}
