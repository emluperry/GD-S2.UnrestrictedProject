using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class CameraMovement : MonoBehaviour
{
    //input
    private PlayerInput _input;

    //rotation input
    private InputAction _cameraInputAction;
    private Vector3 _rotateDirection;

    [Header("External References")]
    [SerializeField] private Transform _playerTransform;

    [Header("Rotation")]
    private Quaternion _initialRotation;
    private float _maxDistFromPlayer;

    [SerializeField][Min(0f)] private float _maxRotateSpeed = 5;
    private Coroutine _rotateCoroutine;
    private bool _isRotating;

    // Start is called before the first frame update
    void Awake()
    {
        _input = GetComponentInParent<PlayerInput>();
        _cameraInputAction = _input.currentActionMap.FindAction("Look");

        _cameraInputAction.performed += Input_LookPerformed;
        _cameraInputAction.canceled += Input_LookCancelled;

        _initialRotation = transform.rotation;
        _maxDistFromPlayer = (_playerTransform.position - transform.position).magnitude;
    }

    #region INPUTS

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

            transform.RotateAround(_playerTransform.position, new Vector3(axis.x, 0, axis.y), -deltaVertical);
        }
    }
}
