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
    private Vector3 _moveDirection;
    private Coroutine _movementCoroutine;
    private bool _isMoving = false;

    //components
    private Rigidbody _rb;

    [Header("External References")]
    [SerializeField] private Transform _cameraTransform;

    [Header("Movement")]
    [SerializeField][Min(0f)] private float _maxSpeed = 5;
    [SerializeField][Min(0f)] private float _maxAccelerationForce = 2;
    [SerializeField][Min(0f)] private float _brakingForce = 5;

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
        Vector3 moveInput = new Vector3(input.x, 0, input.y);

        _moveDirection = _cameraTransform.TransformDirection(moveInput);
        _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }

        if (moveInput.sqrMagnitude > 0 && _movementCoroutine == null)
        {
            _isMoving = true;
            _movementCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        Debug.Log("Move cancelled");
        _moveDirection = Vector3.zero;
        _isMoving = false;

        if(_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _movementCoroutine = StartCoroutine(c_StoppingCoroutine());
    }

    #endregion

    private IEnumerator c_MovementCoroutine()
    {
        while(_isMoving)
        {
            yield return new WaitForFixedUpdate();

            Vector3 maxVelocity = _moveDirection.normalized * _maxSpeed;
            Vector3 deltaVelocity = maxVelocity - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
            deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, _maxAccelerationForce);

            _rb.AddForce(deltaAcceleration, ForceMode.Force);
        }
    }

    private IEnumerator c_StoppingCoroutine()
    {
        while (_rb.velocity.sqrMagnitude >= 1)
        {
            yield return new WaitForFixedUpdate();

            Vector3 deltaVelocity = - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
            deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, _brakingForce);

            _rb.AddForce(deltaAcceleration, ForceMode.Force);
        }
    }
}
