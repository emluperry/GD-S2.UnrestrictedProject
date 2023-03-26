using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

using UI_Enums;

public class UI_Navigation : MonoBehaviour, IInput
{
    //stores layout group that can be toggled with shoulder buttons (or q/e)
    [SerializeField] private HorizontalOrVerticalLayoutGroup[] _shoulderToggleGroups;
    private UI_OnClickButton[] _clickableShoulderObjects;
    private int _currentShoulderIndex = 0;

    //parent transform containing buttons toggled by movement input - could be more efficient, ie by storing a shorter tree on awake, but this is functional
    [SerializeField] private Transform _movementParent;
    private UI_Element _currentUIElement;

    private InputAction _swapInputAction;
    private InputAction _moveInputAction;
    private Vector2 _moveInput;
    private bool _currentlyTakingInput;
    private InputAction _selectInputAction;

    private Coroutine _inputCoroutine;

    [Header("Input Values")]
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
        _currentUIElement = _movementParent.GetComponentInChildren<UI_Element>();
    }

    private void Start()
    {
        if (_clickableShoulderObjects.Length > 0)
            _clickableShoulderObjects[0].ActivateButtonSelection();

        if (_currentUIElement != null)
            _currentUIElement.ActivateButtonSelection();
    }

    #region INPUT SETUP

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _swapInputAction = inputs["Swap"];

        _swapInputAction.performed += Input_SwapPerformed;


        _moveInputAction = inputs["Move"];

        _moveInputAction.performed += Input_MovePerformed;
        _moveInputAction.canceled += Input_MoveCancelled;


        _selectInputAction = inputs["Select"];
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
            _moveInputAction.canceled += Input_MoveCancelled;
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
            _moveInputAction.canceled -= Input_MoveCancelled;
        }

        if (_selectInputAction != null)
        {
            _selectInputAction.performed -= Input_SelectPerformed;
        }
    }

    #endregion

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

        if (swapInput != 0 && _delayCoroutine == null && _inputCoroutine == null && _clickableShoulderObjects.Length > 0)
        {
            //exit old element
            _clickableShoulderObjects[_currentShoulderIndex].DeactivateButtonSelection();

            //update index
            _currentShoulderIndex += swapInput;

            if (_currentShoulderIndex >= _clickableShoulderObjects.Length)
                _currentShoulderIndex = 0;
            else if (_currentShoulderIndex < 0)
                _currentShoulderIndex = _clickableShoulderObjects.Length - 1;

            //update new element
            _clickableShoulderObjects[_currentShoulderIndex].ActivateButtonSelection();
            _clickableShoulderObjects[_currentShoulderIndex].SelectElement();

            //set move button to first in current area - first active button
            _currentUIElement = _movementParent.GetComponentInChildren<UI_Element>();
            if(_currentUIElement != null)
                _currentUIElement.ActivateButtonSelection();

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
        }
    }

    private void Input_MoveCancelled(InputAction.CallbackContext ctx)
    {
        _currentlyTakingInput = false;
    }

    private void Input_MovePerformed(InputAction.CallbackContext ctx)
    {
        _currentlyTakingInput = true;
        _moveInput = ctx.ReadValue<Vector2>();

        if (_moveInput.sqrMagnitude > 0 && _delayCoroutine == null && _inputCoroutine == null)
        {
            _inputCoroutine = StartCoroutine(c_NavigateUI());
        }
    }

    private IEnumerator c_NavigateUI()
    {
        while(_currentlyTakingInput)
        {
            yield return new WaitForFixedUpdate();

            //check if navigation or editing element
            if (_moveInput.x != 0 && _currentUIElement != null && _currentUIElement.selectedElement)
            {
                //update if slider and ignore further inputs
                if (_currentUIElement.type == UI_ELEMENT_TYPE.SLIDER)
                {
                    _currentUIElement.GetComponent<UI_Slider>().IncrementSliderValue(Mathf.RoundToInt(_moveInput.x));
                    _currentUIElement.ActivateButtonSelection();
                }
            }
            else
            {
                //exit old element
                if (_currentUIElement != null)
                {
                    _currentUIElement.DeselectElement();
                    _currentUIElement.DeactivateButtonSelection();
                }

                //find next element based on input
                bool foundNextButton = false;

                //if there is no active element, get first element
                if (_currentUIElement == null)
                {
                    _currentUIElement = _movementParent.GetComponentInChildren<UI_Element>();
                    foundNextButton = _currentUIElement != null;
                }
                else //otherwise check inputs
                {
                    //horizontal input first
                    if (_moveInput.x != 0)
                    {
                        foundNextButton = NavigateToNextNode<HorizontalLayoutGroup>(Mathf.RoundToInt(_moveInput.x));
                    }

                    if (_moveInput.y != 0 && !foundNextButton) //vertical input second, skip if found a button horizontally
                    {
                        foundNextButton = NavigateToNextNode<VerticalLayoutGroup>(Mathf.RoundToInt(-_moveInput.y));
                    }

                }

                //update button - do nothing if none
                if (foundNextButton)
                {
                    _currentUIElement.ActivateButtonSelection();
                }
                else
                    _currentUIElement = null;
            }

            //start timer
            _delayCoroutine = StartCoroutine(c_DelayTimer());
            yield return new WaitUntil(() => _delayCoroutine == null);
        }

        _inputCoroutine = null;
    }

    private bool NavigateToNextNode<T>(int directionIncrement)
    {
        Transform currentNode = _currentUIElement.transform;
        Transform parent = currentNode.parent;

        while (parent != _movementParent.parent)
        {
            if (parent.TryGetComponent(out T layoutGroup))
            {
                int currentSiblingIndex = currentNode.GetSiblingIndex();
                int siblingIndex = currentSiblingIndex;

                //check each child in order
                do
                {
                    siblingIndex += directionIncrement;

                    if (siblingIndex >= parent.childCount)
                        siblingIndex = 0;
                    else if (siblingIndex < 0)
                        siblingIndex = parent.childCount - 1;

                    UI_Element tempElement = parent.GetChild(siblingIndex).GetComponentInChildren<UI_Element>();
                    if (tempElement != null)
                    {
                        _currentUIElement = tempElement;
                        break;
                    }
                }
                while (siblingIndex != currentSiblingIndex);

                return _currentUIElement != null;
            }
            else
            {
                currentNode = parent;
                parent = currentNode.parent;
            }
        }

        return false;
    }

    private void Input_SelectPerformed(InputAction.CallbackContext ctx)
    {
        if(ctx.ReadValueAsButton() && _currentUIElement != null && _currentUIElement.gameObject.activeInHierarchy)
        {
            _currentUIElement.SelectElement();
        }
    }
}
