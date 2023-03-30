using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour, IInput
{
    //movement input
    private InputAction _moveInputAction;
    private InputAction _jumpInputAction;

    //rotation input
    private InputAction _cameraInputAction;
    private Vector3 _rotateDirection;

    //targeting
    private Transform _targetTransform;
    Coroutine _targetCoroutine;

    [Header("External References")]
    [SerializeField] private Transform _playerTransform;
    private Rigidbody _playerRb;
    private PlayerJump _playerJumpComponent;

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
    [SerializeField][Min(0f)] private float _autoRotateSpeed = 20;
    [SerializeField][Min(0f)] private float _forwardRotateSpeed = 90;
    private Coroutine _rotateCoroutine;
    private bool _isRotating = false;

    void Awake()
    {
        _playerTransform.GetComponent<PlayerTargeting>().onTargetChanged += SetTarget;
        _playerJumpComponent = _playerTransform.GetComponent<PlayerJump>();
        _playerRb = _playerTransform.GetComponent<Rigidbody>();
    }

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _moveInputAction = inputs["Move"];
        _jumpInputAction = inputs["Jump"];
        _cameraInputAction = inputs["Look"];

        _moveInputAction.performed += Input_MovePerformed;
        _moveInputAction.canceled += Input_MoveCancelled;

        _jumpInputAction.performed += Input_JumpPerformed;
        _jumpInputAction.canceled += Input_JumpCancelled;

        _cameraInputAction.performed += Input_LookPerformed;
        _cameraInputAction.canceled += Input_LookCancelled;
    }

    private void OnDestroy()
    {
        if(_moveInputAction != null)
        {
            _moveInputAction.performed -= Input_MovePerformed;
            _moveInputAction.canceled -= Input_MoveCancelled;

            _jumpInputAction.performed -= Input_JumpPerformed;
            _jumpInputAction.canceled -= Input_JumpCancelled;

            _cameraInputAction.performed -= Input_LookPerformed;
            _cameraInputAction.canceled -= Input_LookCancelled;
        }
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

    private void Input_JumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_moveCoroutine != null)
        {
            StopCoroutine(_moveCoroutine);
            _moveCoroutine = null;
        }

        if (ctx.ReadValueAsButton())
        {
            _isMoving = true;
            _moveCoroutine = StartCoroutine(c_FollowCoroutine());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        _isMoving = false;
    }
    
    private void Input_JumpCancelled(InputAction.CallbackContext ctx)
    {
        _isMoving = false;
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
        while (_isMoving || !_playerJumpComponent.GetIsGrounded() || _playerRb.velocity.sqrMagnitude > Mathf.Epsilon)
        {
            yield return new WaitForFixedUpdate();

            //move to player
            oldPos = transform.position;
            transform.position = _playerTransform.position - transform.forward * _maxDistFromPlayer;

            _deltaDistance = Vector3.Distance(transform.position, oldPos);

            //if not facing direction of movement, rotate slowly behind character
            Vector2 CameraDirectionValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
            Vector2 PlayerDirectionValues = new Vector2(_playerTransform.forward.x, _playerTransform.forward.z).normalized;

            float totalAngle = Vector2.SignedAngle(CameraDirectionValues, PlayerDirectionValues) * -1;

            if (totalAngle < 0)
                totalAngle = Mathf.Clamp(totalAngle, -_autoRotateSpeed * Time.fixedDeltaTime , 0);
            else
                totalAngle = Mathf.Clamp(totalAngle, 0, _autoRotateSpeed * Time.fixedDeltaTime);

            RotateHorizontally(Mathf.Min(_autoRotateSpeed, totalAngle));
        }

        _moveCoroutine = StartCoroutine(c_FocusCoroutine());
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

            RotateHorizontally(_maxRotateSpeed * _rotateDirection.x * Time.fixedDeltaTime);

            //handle vertical rotation

            float deltaVertical = _maxRotateSpeed * Time.fixedDeltaTime * _rotateDirection.y;

            RotateVertically(deltaVertical);
        }
    }

    private IEnumerator c_ForwardFocusCoroutine()
    {
        Vector2 CameraDirectionValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 PlayerDirectionValues = new Vector2(_playerTransform.forward.x, _playerTransform.forward.z).normalized;

        float totalAngle = Vector2.SignedAngle(CameraDirectionValues, PlayerDirectionValues) * -1;

        while (Mathf.Abs(totalAngle) > 1)
        {
            if (totalAngle < 0)
                totalAngle = Mathf.Clamp(totalAngle, -_forwardRotateSpeed * Time.fixedDeltaTime, 0);
            else
                totalAngle = Mathf.Clamp(totalAngle, 0, _forwardRotateSpeed * Time.fixedDeltaTime);

            RotateHorizontally(totalAngle);

            yield return new WaitForFixedUpdate();

            CameraDirectionValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
            PlayerDirectionValues = new Vector2(_playerTransform.forward.x, _playerTransform.forward.z).normalized;

            totalAngle = Vector2.SignedAngle(CameraDirectionValues, PlayerDirectionValues) * -1;
        }
    }

    private IEnumerator c_TargetFocusCoroutine()
    {
        Vector2 CameraDirectionValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector3 direction = (_targetTransform.position - _playerTransform.position).normalized;
        Vector2 FocusDirectionValues = new Vector2(direction.x, direction.z);

        float totalAngle = Vector2.SignedAngle(CameraDirectionValues, FocusDirectionValues) * -1;

        while (true)
        {
            //rotate horizontally
            if (totalAngle < 0)
                totalAngle = Mathf.Clamp(totalAngle, -_forwardRotateSpeed * Time.fixedDeltaTime, 0);
            else
                totalAngle = Mathf.Clamp(totalAngle, 0, _forwardRotateSpeed * Time.fixedDeltaTime);

            RotateHorizontally(totalAngle);

            yield return new WaitForFixedUpdate();

            CameraDirectionValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
            direction = (_targetTransform.position - _playerTransform.position).normalized;
            FocusDirectionValues = new Vector2(direction.x, direction.z);

            totalAngle = Vector2.SignedAngle(CameraDirectionValues, FocusDirectionValues) * -1;
        }
    }

    private void RotateVertically(float deltaVertical)
    {
        //calculate axis - perpendicular to forward vector
        Vector2 xzValues = new Vector2(transform.forward.x, transform.forward.z).normalized;
        Vector2 axis = Vector2.Perpendicular(xzValues);

        //clamp within boundaries
        float diff = 0;
        if (transform.eulerAngles.x - deltaVertical > 90 - _maxRotationBoundary)
        {
            diff = (transform.eulerAngles.x - deltaVertical) - (90 - _maxRotationBoundary);
            deltaVertical -= diff;
        }
        else if (transform.eulerAngles.x - deltaVertical < _maxRotationBoundary)
        {
            diff = _maxRotationBoundary - (transform.eulerAngles.x - deltaVertical);
            deltaVertical += diff;
        }

        transform.RotateAround(_playerTransform.position, new Vector3(axis.x, 0, axis.y), -deltaVertical);
    }

    private void RotateHorizontally(float maxAngle)
    {
        transform.RotateAround(_playerTransform.position, Vector3.up, maxAngle);
    }

    private void SetTarget(Transform target)
    {
        if (target == null)
        {
            _targetTransform = null;
            if(_targetCoroutine != null)
            {
                StopCoroutine(_targetCoroutine);
                _targetCoroutine = null;
            }

            _targetCoroutine = StartCoroutine(c_ForwardFocusCoroutine());
        }
        else
        {
            _targetTransform = target;

            if (_targetCoroutine != null)
            {
                StopCoroutine(_targetCoroutine);
                _targetCoroutine = null;
            }

            _targetCoroutine = StartCoroutine(c_TargetFocusCoroutine());
        }
    }
}
