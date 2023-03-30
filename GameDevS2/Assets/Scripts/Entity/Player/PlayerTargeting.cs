using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerTargeting : MonoBehaviour, IInput, IPausable
{
    private List<EnemyInitialiser> _targetArray = new List<EnemyInitialiser>();
    private int _currentTargetIndex = -1;

    //target input
    private InputAction _targetInputAction;
    private bool _isTargetPressed;

    [Header("Targeting")]
    [SerializeField] private float _targetSwapMaxDelay = 1f;
    private float _currentSwapDelay = 1f;

    public Action<Transform> onTargetChanged;

    private bool _isPaused = false;

    private void Awake()
    {
        _currentSwapDelay = _targetSwapMaxDelay;
    }

    #region INPUT SETUP

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _targetInputAction = inputs["Target"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_targetInputAction != null)
        {
            _targetInputAction.performed += Input_TargetPerformed;
            _targetInputAction.canceled += Input_TargetCancelled;
        }
    }

    public void DisableInput()
    {
        if (_targetInputAction != null)
        {
            _targetInputAction.performed += Input_TargetPerformed;
            _targetInputAction.canceled += Input_TargetCancelled;
        }
    }

    #endregion

    private void OnDestroy()
    {
        DisableInput();
    }

    #region INPUTS

    private void Input_TargetPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

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
        if (_isPaused)
            return;

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
            onTargetChanged?.Invoke(_targetArray[_currentTargetIndex].transform);
        }

        _currentSwapDelay = 0;

        while(_currentSwapDelay < _targetSwapMaxDelay)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            _currentSwapDelay += Time.fixedDeltaTime;
        }
    }

    public void SetEnemyList(EnemyInitialiser[] enemy)
    {
        ClearTargetList();

        _targetArray = new List<EnemyInitialiser>();

        for(int i = 0; i < enemy.Length; i++)
        {
            _targetArray.Add(enemy[i]);
            enemy[i].health.onDead += RemoveEnemy;
        }
    }

    private void RemoveEnemy(EntityHealth enemyHealth)
    {
        enemyHealth.onDead -= RemoveEnemy;

        _targetArray.Remove(enemyHealth.GetComponent<EnemyInitialiser>());

        Mathf.Clamp(_currentTargetIndex, 0, _targetArray.Count - 1);

        if(_targetArray.Count <= 0)
        {
            _currentTargetIndex = -1;
            onTargetChanged?.Invoke(null);
            return;
        }

        onTargetChanged?.Invoke(_targetArray[_currentTargetIndex].transform);
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

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }
}
