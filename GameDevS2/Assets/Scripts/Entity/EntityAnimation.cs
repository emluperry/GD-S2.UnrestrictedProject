using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour
{
    private Animator _animator;

    private float _maxMovementSpeed = 0;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
    }

    public void SetupValues(float maxSpeed)
    {
        _maxMovementSpeed = maxSpeed;
    }

    public void UpdateSpeed(float currentSpeed)
    {
        _animator.SetFloat("Speed_f", currentSpeed / _maxMovementSpeed);
    }
}
