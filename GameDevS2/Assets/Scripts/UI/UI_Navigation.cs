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

    //parent transform containing buttons toggled by movement input - could be more efficient, ie by storing a shorter tree on awake, but this is functional
    [SerializeField] private Transform _movementParent;
    private UI_OnClickButton _currentMoveButton;

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
            shoulderButtonList.AddRange(group.GetComponentsInChildren<UI_OnClickButton>(true));
        }

        _clickableShoulderObjects = new UI_OnClickButton[shoulderButtonList.Count];
        shoulderButtonList.CopyTo(0, _clickableShoulderObjects, 0, shoulderButtonList.Count);

        //move buttons
        _currentMoveButton = _movementParent.GetComponentInChildren<UI_OnClickButton>();
    }

    private void Start()
    {
        if (_clickableShoulderObjects.Length > 0)
            _clickableShoulderObjects[0].ActivateButtonSelection();

        if (_currentMoveButton != null)
            _currentMoveButton.ActivateButtonSelection();
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

            //set move button to first in current area - first active button
            _currentMoveButton = _movementParent.GetComponentInChildren<UI_OnClickButton>();

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
        }
    }

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        Vector2 input = ctx.ReadValue<Vector2>();

        if (input.sqrMagnitude > 0 && _delayCoroutine == null)
        {
            //exit old button
            if (_currentMoveButton != null)
                _currentMoveButton.DeactivateButtonSelection();

            //find next button based on input
            bool foundNextButton = false;

            //if there is no active button, get first button
            if (_currentMoveButton == null)
            {
                _currentMoveButton = _movementParent.GetComponentInChildren<UI_OnClickButton>();
                foundNextButton = _currentMoveButton != null;
            }
            else //otherwise check inputs
            {
                //horizontal input first
                if (input.x != 0)
                {
                    Transform currentNode = _currentMoveButton.transform;
                    Transform parent = currentNode.parent;

                    while (parent != _movementParent.parent)
                    {
                        if (parent.TryGetComponent(out HorizontalLayoutGroup layoutGroup))
                        {
                            int siblingIndex = currentNode.GetSiblingIndex();
                            siblingIndex += Mathf.RoundToInt(input.x);

                            if (siblingIndex >= parent.childCount)
                                siblingIndex = 0;
                            else if (siblingIndex < 0)
                                siblingIndex = parent.childCount - 1;

                            _currentMoveButton = parent.GetChild(siblingIndex).GetComponentInChildren<UI_OnClickButton>();
                            foundNextButton = true;
                            break;
                        }
                        else
                        {
                            currentNode = parent;
                            parent = currentNode.parent;
                        }
                    }
                }
                
                if (input.y != 0 && !foundNextButton) //vertical input second, skip if found a button horizontally
                {
                    Transform currentNode = _currentMoveButton.transform;
                    Transform parent = currentNode.parent;

                    while (parent != _movementParent.parent)
                    {
                        if (parent.TryGetComponent(out VerticalLayoutGroup layoutGroup))
                        {
                            int siblingIndex = currentNode.GetSiblingIndex();
                            siblingIndex += Mathf.RoundToInt(-input.y);

                            if (siblingIndex >= parent.childCount)
                                siblingIndex = 0;
                            else if (siblingIndex < 0)
                                siblingIndex = parent.childCount - 1;

                            _currentMoveButton = parent.GetChild(siblingIndex).GetComponentInChildren<UI_OnClickButton>();
                            foundNextButton = true;
                            break;
                        }
                        else
                        {
                            currentNode = parent;
                            parent = currentNode.parent;
                        }
                    }
                }

            }

            //update button - do nothing if none
            if (foundNextButton)
            {
                _currentMoveButton.ActivateButtonSelection();
            }
            else
                _currentMoveButton = null;

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
        }
    }

    private void Input_SelectPerformed(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValueAsButton() && _currentMoveButton != null && _currentMoveButton.gameObject.activeInHierarchy)
        {
            _currentMoveButton.ClickButton();
        }
    }
}
