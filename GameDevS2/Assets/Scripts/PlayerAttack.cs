using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    //input
    private PlayerInput _input;

    //jump input
    private InputAction _attackInputAction;
    private bool _isAttackPressed;

    private Coroutine _attackingCoroutine;

    [Header("Attacking")]
    [SerializeField][Min(0f)] private float _maxAttackDelay = 0.5f;
    private float _currentAttackDelay = 0;
    //[SerializeField][Min(0f)] private float _minJumpForce = 3500f; //min height: 3.5? max height: 5?
    //[SerializeField][Min(0f)] private float _extraJumpForce = 50f;
    //[SerializeField][Min(0f)] private float _maxButtonHoldDuration = 1f;
    private EntityHealth _targetHealth;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _attackInputAction = _input.currentActionMap.FindAction("Attack");

        _attackInputAction.performed += Input_AttackPerformed;
        _attackInputAction.canceled += Input_AttackCancelled;
    }

    #region INPUTS

    private void Input_AttackPerformed(InputAction.CallbackContext ctx)
    {
        _isAttackPressed = ctx.ReadValueAsButton();

        if(_attackingCoroutine == null && _isAttackPressed)
        {
            Debug.Log("Attacking...");
            _targetHealth = FindObjectOfType<EntityHealth>();
            _attackingCoroutine = StartCoroutine(c_AttackingCoroutine());
        }
    }

    private void Input_AttackCancelled(InputAction.CallbackContext ctx)
    {
        _isAttackPressed = false;
    }

    #endregion

    private IEnumerator c_AttackingCoroutine()
    {
        yield return new WaitForFixedUpdate();

        if(_targetHealth != null)
            _targetHealth.TakeDamage(5);

        while(_currentAttackDelay < _maxAttackDelay)
        {
            _currentAttackDelay += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _currentAttackDelay = 0;
        _attackingCoroutine = null;
    }
}
