using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _input;

    //input
    private InputAction _moveInputAction;
    private Vector2 _moveInput;
    private Coroutine _movementCoroutine;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _moveInputAction = _input.currentActionMap.FindAction("Move");

        _moveInputAction.performed += Input_MovePerformed;
        _moveInputAction.canceled += Input_MoveCancelled;
    }

    #region INPUTS

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();

        if(_moveInput.sqrMagnitude > 0)
        {
            _movementCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        _moveInput = ctx.ReadValue<Vector2>();

        StopCoroutine(_movementCoroutine);
    }

    #endregion

    private IEnumerator c_MovementCoroutine()
    {
        yield return null;
    }
}
