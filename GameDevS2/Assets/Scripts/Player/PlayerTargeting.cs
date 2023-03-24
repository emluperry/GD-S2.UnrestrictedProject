using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargeting : MonoBehaviour, IInput
{
    private List<GameObject> _targetArray = new List<GameObject>();
    private int _currentTargetIndex = -1;

    //target input
    private InputAction _targetInputAction;
    private bool _isTargetPressed;

    [Header("Targeting")]
    [SerializeField] private float _targetSwapMaxDelay = 1f;
    private float _currentSwapDelay = 1f;

    public Action<GameObject> onTargetChanged;

    private void Awake()
    {
        _currentSwapDelay = _targetSwapMaxDelay;
    }

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _targetInputAction = inputs["Target"];

        _targetInputAction.performed += Input_TargetPerformed;
        _targetInputAction.canceled += Input_TargetCancelled;
    }

    #region INPUTS

    private void Input_TargetPerformed(InputAction.CallbackContext ctx)
    {
        _isTargetPressed = ctx.ReadValueAsButton();

        if (_isTargetPressed && _currentSwapDelay >= _targetSwapMaxDelay)
        {
            if(_targetArray.Count > 0)
            {
                _currentTargetIndex++;

                if (_currentTargetIndex >= _targetArray.Count)
                {
                    _currentTargetIndex = -1;
                }
            }

            StartCoroutine(c_TargetSwapCoroutine());
        }
    }

    private void Input_TargetCancelled(InputAction.CallbackContext ctx)
    {
        _isTargetPressed = false;
    }

    #endregion

    private IEnumerator c_TargetSwapCoroutine()
    {
        //set other components
        if(_currentTargetIndex == -1)
            onTargetChanged?.Invoke(null);
        else
        {
            onTargetChanged?.Invoke(_targetArray[_currentTargetIndex]);
        }

        _currentSwapDelay = 0;

        while(_currentSwapDelay < _targetSwapMaxDelay)
        {
            yield return new WaitForFixedUpdate();

            _currentSwapDelay += Time.fixedDeltaTime;
        }
    }

    public void SetEnemyList(EntityHealth[] enemyHealth)
    {
        ClearTargetList();
        _targetArray = new List<GameObject>();

        for(int i = 0; i < enemyHealth.Length; i++)
        {
            _targetArray.Add(enemyHealth[i].gameObject);
            enemyHealth[i].onDead += RemoveEnemy;
        }
    }

    private void RemoveEnemy()
    {
        for(int i = 0; i < _targetArray.Count; i++)
        {
            if (_targetArray[i].TryGetComponent(out EntityHealth health) && health.GetIsDead())
            {
                health.onDead -= RemoveEnemy;
                _targetArray.Remove(_targetArray[i]);

                if(_currentTargetIndex >= _targetArray.Count)
                {
                    if(_targetArray.Count > 0)
                    {
                        _currentTargetIndex = _targetArray.Count - 1;
                        onTargetChanged?.Invoke(_targetArray[_currentTargetIndex]);
                    }
                    else
                    {
                        _currentTargetIndex = -1;
                        onTargetChanged?.Invoke(null);
                    }
                }
            }
        }
    }

    private void ClearTargetList()
    {
        for (int i = 0; i < _targetArray.Count; i++)
        {
            if (_targetArray[i].TryGetComponent(out EntityHealth health))
            {
                health.onDead -= RemoveEnemy;
            }
            _targetArray.Remove(_targetArray[i]);
        }

        _targetArray.Clear();

        _currentTargetIndex = -1;
        onTargetChanged?.Invoke(null);
    }
}
