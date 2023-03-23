using Scene_Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PauseHandler : MonoBehaviour
{
    private PlayerInput _input;

    private bool _pauseInputFlag = true;
    private bool _isPaused = false;

    public Action<bool> onLoadPause;

    public void Awake()
    {
        _input = GetComponent<PlayerInput>();

        _input.currentActionMap.FindAction("Pause").performed += Handle_Pause_Performed;
        _input.currentActionMap.FindAction("Pause").canceled += Handle_Pause_Cancelled;
    }

    private void Handle_Pause_Performed(InputAction.CallbackContext context)
    {
        if(_pauseInputFlag && context.ReadValueAsButton())
        {
            _isPaused = !_isPaused;
            onLoadPause?.Invoke(_isPaused);
            _pauseInputFlag = false;
        }
    }

    private void Handle_Pause_Cancelled(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
            _pauseInputFlag = true;
    }
}
