using Scene_Enums;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class UI_PauseHandler : MonoBehaviour
{
    private bool _pauseInputFlag = true;
    private bool _isPaused = false;

    public Action<bool> onLoadPause;

    public void SetInputActions(Dictionary<string, InputAction> inputActions)
    {
        inputActions["Pause"].performed += Handle_Pause_Performed;
        inputActions["Pause"].canceled += Handle_Pause_Cancelled;
    }

    private void Handle_Pause_Performed(InputAction.CallbackContext context)
    {
        if(_pauseInputFlag && context.ReadValueAsButton())
        {
            TogglePause();
            _pauseInputFlag = false;
        }
    }

    private void Handle_Pause_Cancelled(InputAction.CallbackContext context)
    {
        if (!context.ReadValueAsButton())
            _pauseInputFlag = true;
    }

    public void TogglePause()
    {
        _isPaused = !_isPaused;
        onLoadPause?.Invoke(_isPaused);
    }
}
