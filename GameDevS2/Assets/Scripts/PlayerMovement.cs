using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    //input
    private PlayerInput _input;
    //movement input
    private InputAction _moveInputAction;
    private Vector3 _moveInput;
    private Coroutine _movementCoroutine;
    private bool _isMoving = false;

    //components
    private Rigidbody _rb;

    [Header("Movement")]
    [SerializeField][Min(0f)] private float _maxSpeed = 5;
    [SerializeField][Min(0f)] private float _maxAccelerationForce;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _moveInputAction = _input.currentActionMap.FindAction("Move");

        _moveInputAction.performed += Input_MovePerformed;
        _moveInputAction.canceled += Input_MoveCancelled;

        _rb = GetComponent<Rigidbody>();
    }

    #region INPUTS

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        _moveInput = new Vector3(input.x, 0, input.y);

        if(_moveInput.sqrMagnitude > 0)
        {
            _isMoving = true;
            _movementCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        _moveInput = Vector3.zero;
        _isMoving = false;

        if(_movementCoroutine != null)
            StopCoroutine(_movementCoroutine);
    }

    #endregion

    private IEnumerator c_MovementCoroutine()
    {
        while(_isMoving)
        {
            yield return new WaitForFixedUpdate();

            _rb.AddForce(_moveInput * _maxSpeed, ForceMode.Force);
        }
    }
}
