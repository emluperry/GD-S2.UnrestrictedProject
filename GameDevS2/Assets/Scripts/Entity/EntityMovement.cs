using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityMovement : MonoBehaviour, IPausable
{
    protected Vector3 _moveInput;
    protected Vector3 _moveDirection;

    protected Coroutine _movementCoroutine;
    protected bool _isMoving = false;

    //components
    protected Rigidbody _rb;

    [Header("Movement")]
    [SerializeField][Min(0f)] protected float _maxSpeed = 5;
    [SerializeField][Min(0f)] protected float _maxAccelerationForce = 2;
    [SerializeField][Min(0f)] protected float _brakingForce = 5;

    [Header("Rotation")]
    [SerializeField][Min(0f)] protected float _maxRotationSpeed = 5;
    [SerializeField][Min(0f)] protected float _rotationDampener = 3;

    protected bool _isPaused = false;

    protected virtual void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public float GetMaxSpeed()
    {
        return _maxSpeed;
    }

    protected virtual IEnumerator c_MovementCoroutine()
    {
        while (_isMoving)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            MoveEntity();

            RotateEntity();
        }
    }

    protected virtual void MoveEntity()
    {
        //update velocity
        Vector3 maxVelocity = _moveDirection.normalized * _maxSpeed;
        Vector3 deltaVelocity = maxVelocity - new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

        Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
        deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, _maxAccelerationForce);

        _rb.AddForce(deltaAcceleration, ForceMode.Force);
    }

    protected virtual void RotateEntity()
    {
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

    protected IEnumerator c_StoppingCoroutine()
    {
        while (_rb.velocity.sqrMagnitude >= 1)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            Vector3 deltaVelocity = -new Vector3(_rb.velocity.x, 0, _rb.velocity.z);

            Vector3 deltaAcceleration = deltaVelocity / Time.fixedDeltaTime;
            deltaAcceleration = Vector3.ClampMagnitude(deltaAcceleration, _brakingForce);

            _rb.AddForce(deltaAcceleration, ForceMode.Force);
        }
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }
}
