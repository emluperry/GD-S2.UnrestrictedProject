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

    [Header("Rotation")]
    [SerializeField][Min(0f)] private float _maxRotationSpeed = 5;
    [SerializeField][Min(0f)] private float _rotationDampener = 3;

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

    private IEnumerator c_MovementCoroutine()
    {
        while(_isMoving)
        {
            yield return new WaitForFixedUpdate();

            //update input direction
            _moveDirection = _cameraTransform.TransformDirection(_moveInput);
            _moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);

            //update velocity
            Vector3 maxVelocity = _moveDirection.normalized * _maxSpeed;
            Vector3 deltaVelocity = maxVelocity - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
            deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, _maxAccelerationForce);

            _rb.AddForce(deltaAcceleration, ForceMode.Force);

            //turn to input direction

            Quaternion targetRotation = Quaternion.LookRotation(_moveDirection, Vector3.up);
            Quaternion toGoal = targetRotation * Quaternion.Inverse(transform.rotation);

            Vector3 rotAxis;
            float rotDegrees;
            toGoal.ToAngleAxis(out rotDegrees, out rotAxis);
            rotAxis.Normalize();

            rotDegrees -= (rotDegrees > 180) ? 360 : 0;

            float rotRadians = rotDegrees * Mathf.Deg2Rad;

            _rb.AddTorque((rotAxis * (rotRadians * _maxRotationSpeed * Time.fixedDeltaTime)) - (_rb.angularVelocity * _rotationDampener));
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
