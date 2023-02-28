using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    //input
    private PlayerInput _input;

    //movement input
    private InputAction _moveInputAction;

    //rotation input
    private InputAction _cameraInputAction;
    private Vector3 _rotateDirection;

    [Header("External References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Movement")]
    [SerializeField][Min(0f)] private float _maxDistFromPlayer = 5;
    [SerializeField][Min(0f)] private float _decelerationFactor = 0.9f;
    [SerializeField][Min(0f)] private float _focusSpeed = 0.5f;
    private float _deltaDistance = 0;
    private Coroutine _moveCoroutine;
    private bool _isMoving = false;

    [Header("Rotation")]
    [SerializeField][Min(0f)] private float _maxRotationBoundary = 20;
    [SerializeField][Min(0f)] private float _maxRotateSpeed = 5;
    private Coroutine _rotateCoroutine;
    private bool _isRotating = false;

    // Start is called before the first frame update
    void Awake()
    {
        _input = _playerTransform.GetComponent<PlayerInput>();
        _moveInputAction = _input.currentActionMap.FindAction("Move");
        _cameraInputAction = _input.currentActionMap.FindAction("Look");

        _moveInputAction.performed += Input_MovePerformed;
        _moveInputAction.canceled += Input_MoveCancelled;

        _cameraInputAction.performed += Input_LookPerformed;
        _cameraInputAction.canceled += Input_LookCancelled;
    }

    #region INPUTS

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        if(_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        if(ctx.ReadValue<Vector2>().sqrMagnitude > 0)
        {
            _isMoving = true;
            _moveCoroutine = StartCoroutine(c_FollowCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        _isMoving = false;

        if(_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        _moveCoroutine = StartCoroutine(c_FocusCoroutine());
    }

    private void Input_LookPerformed(InputAction.CallbackContext ctx)
    {
        _rotateDirection = ctx.ReadValue<Vector2>();

        if (_rotateDirection.sqrMagnitude > 0 && _rotateCoroutine == null)
        {
            _isRotating = true;
            _rotateCoroutine = StartCoroutine(c_RotateCoroutine());
        }
    }

    private void Input_LookCancelled(InputAction.CallbackContext ctx)
    {
        _rotateDirection = ctx.ReadValue<Vector2>();

        _rotateDirection = Vector3.zero;
        _isRotating = false;

        if (_rotateCoroutine != null)
        {
            StopCoroutine(_rotateCoroutine);
            _rotateCoroutine = null;
        }
    }

    #endregion

    private IEnumerator c_FollowCoroutine()
    {
        Vector3 oldPos;
        while(_isMoving)
        {
            yield return new WaitForFixedUpdate();

            oldPos = transform.position;
            transform.position = _playerTransform.position - transform.forward * _maxDistFromPlayer;

            _deltaDistance = Vector3.Distance(transform.position, oldPos);
        }
    }

    private IEnumerator c_FocusCoroutine()
    {
        yield return new WaitForFixedUpdate();

        float minDeltaDistance = _focusSpeed * Time.fixedDeltaTime;

        while (Vector3.Distance(transform.position, _playerTransform.position - transform.forward * _maxDistFromPlayer) > 0.01f)
        {
            yield return new WaitForFixedUpdate();

            _deltaDistance *= _decelerationFactor;

            transform.position = Vector3.MoveTowards(transform.position, _playerTransform.position - transform.forward * _maxDistFromPlayer, Mathf.Max(_deltaDistance, minDeltaDistance));
        }

        transform.position = _playerTransform.position - transform.forward * _maxDistFromPlayer;
        _deltaDistance = 0;
    }

    private IEnumerator c_RotateCoroutine()
    {
        while (_isRotating)
        {
            yield return new WaitForFixedUpdate();

            //handle horizontal rotation

            float deltaHorizontal = _maxRotateSpeed * Time.fixedDeltaTime * _rotateDirection.x;

            transform.RotateAround(_playerTransform.position, Vector3.up, deltaHorizontal);

            //handle vertical rotation

            float deltaVertical = _maxRotateSpeed * Time.fixedDeltaTime * _rotateDirection.y;

            //calculate axis - perpendicular to forward vector
            Vector2 xzValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
            Vector2 axis = Vector2.Perpendicular(xzValues);

            float diff = 0;
            if(transform.eulerAngles.x - deltaVertical > 90 - _maxRotationBoundary)
            {
                diff = (transform.eulerAngles.x - deltaVertical) - (90 - _maxRotationBoundary);
                deltaVertical -= diff;
            }
            else if(transform.eulerAngles.x - deltaVertical < _maxRotationBoundary)
            {
                diff = _maxRotationBoundary - (transform.eulerAngles.x - deltaVertical);
                deltaVertical += diff;
            }
                
            transform.RotateAround(_playerTransform.position, new Vector3(axis.x, 0, axis.y), -deltaVertical);
        }
    }
}
