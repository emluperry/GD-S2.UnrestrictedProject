using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class UI_Navigation : MonoBehaviour, IInput
{
    //stores layout group that can be toggled with shoulder buttons (or q/e)
    [SerializeField] private HorizontalOrVerticalLayoutGroup[] _shoulderToggleGroups;
    private UI_OnClickButton[] _clickableShoulderObjects;
    private int _currentShoulderIndex = 0;

    //stores layout group(s) that can be toggled with movement input
    [SerializeField] private HorizontalOrVerticalLayoutGroup[] _movementToggleGroups;
    private UI_OnClickButton[] _clickableMoveObjects;
    private int _currentMoveIndex = 0;

    private InputAction _swapInputAction;
    private InputAction _moveInputAction;
    private InputAction _selectInputAction;

    [SerializeField] private float _inputDelay = 0.05f;
    private Coroutine _delayCoroutine;

    private void Awake()
    {
        //shoulder buttons
        List<UI_OnClickButton> shoulderButtonList = new List<UI_OnClickButton>();

        foreach(HorizontalOrVerticalLayoutGroup group in _shoulderToggleGroups)
        {
            shoulderButtonList.AddRange(group.GetComponentsInChildren<UI_OnClickButton>());
        }

        _clickableShoulderObjects = new UI_OnClickButton[shoulderButtonList.Count];
        shoulderButtonList.CopyTo(0, _clickableShoulderObjects, 0, shoulderButtonList.Count);

        if (_clickableShoulderObjects.Length > 0)
            _clickableShoulderObjects[0].ActivateButtonSelection();

        //move buttons
        List<UI_OnClickButton> moveButtonList = new List<UI_OnClickButton>();

        foreach (HorizontalOrVerticalLayoutGroup group in _movementToggleGroups)
        {
            moveButtonList.AddRange(group.GetComponentsInChildren<UI_OnClickButton>());
        }

        _clickableMoveObjects = new UI_OnClickButton[moveButtonList.Count];
        moveButtonList.CopyTo(0, _clickableMoveObjects, 0, moveButtonList.Count);

        if (_clickableMoveObjects.Length > 0)
            _clickableMoveObjects[0].ActivateButtonSelection();
    }

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _swapInputAction = inputs["Swap"];

        _swapInputAction.performed += Input_SwapPerformed;


        _moveInputAction = inputs["Move"];

        _moveInputAction.performed += Input_MovePerformed;


        _selectInputAction = inputs["Attack"];
        _selectInputAction.performed += Input_SelectPerformed;
    }

    public void SetupInput()
    {
        //only use if input actions have been previously saved
        if(_swapInputAction != null)
        {
            _swapInputAction.performed += Input_SwapPerformed;
        }

        if(_moveInputAction != null)
        {
            _moveInputAction.performed += Input_MovePerformed;
        }

        if(_selectInputAction != null)
        {
            _selectInputAction.performed += Input_SelectPerformed;
        }
    }

    public void DisableInput()
    {
        if (_swapInputAction != null)
        {
            _swapInputAction.performed -= Input_SwapPerformed;
        }

        if (_moveInputAction != null)
        {
            _moveInputAction.performed -= Input_MovePerformed;
        }

        if (_selectInputAction != null)
        {
            _selectInputAction.performed -= Input_SelectPerformed;
        }
    }

    private IEnumerator c_DelayTimer()
    {
        float currentTime = 0;
        while(currentTime < _inputDelay)
        {
            yield return new WaitForFixedUpdate();
            currentTime += Time.fixedDeltaTime;
        }

        _delayCoroutine = null;
    }

    private void Input_SwapPerformed(InputAction.CallbackContext ctx)
    {
        float input = ctx.ReadValue<float>();
        int swapInput = Mathf.RoundToInt(input);

        if (swapInput != 0 && _delayCoroutine == null)
        {
            //exit old button
            _clickableShoulderObjects[_currentShoulderIndex].DeactivateButtonSelection();

            //update index
            _currentShoulderIndex += swapInput;

            if (_currentShoulderIndex >= _clickableShoulderObjects.Length)
                _currentShoulderIndex = 0;
            else if (_currentShoulderIndex < 0)
                _currentShoulderIndex = _clickableShoulderObjects.Length - 1;

            //update new button
            _clickableShoulderObjects[_currentShoulderIndex].ActivateButtonSelection();
            _clickableShoulderObjects[_currentShoulderIndex].ClickButton();

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
        }
    }

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();
        int moveInput = Mathf.RoundToInt(input.y); //only doing vertical movement for now, would like to look into horizontal

        if (moveInput != 0 && _delayCoroutine == null)
        {
            //exit old button
            _clickableMoveObjects[_currentMoveIndex].DeactivateButtonSelection();

            //update index
            _currentMoveIndex += moveInput;

            if (_currentMoveIndex >= _clickableMoveObjects.Length)
                _currentMoveIndex = 0;
            else if (_currentMoveIndex < 0)
                _currentMoveIndex = _clickableMoveObjects.Length - 1;

            //update new button
            _clickableMoveObjects[_currentMoveIndex].ActivateButtonSelection();

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
        }
    }

    private void Input_SelectPerformed(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValueAsButton())
        {
            _clickableMoveObjects[_currentMoveIndex].ClickButton();
        }
    }
}
