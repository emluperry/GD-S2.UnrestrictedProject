using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMovement : EntityMovement
{
    public void StartMovement(Vector3 direction)
    {
        if (_isPaused)
            return;

        _moveInput = direction;
        _moveDirection = direction;

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

    public void StopMovement()
    {
        if (_isPaused)
            return;

        _moveInput = Vector3.zero;
        _rb.angularVelocity = Vector3.zero;
        _isMoving = false;

        if (_movementCoroutine != null)
        {
            StopCoroutine(_movementCoroutine);
            _movementCoroutine = null;
        }
        _movementCoroutine = StartCoroutine(c_StoppingCoroutine());
    }
}
