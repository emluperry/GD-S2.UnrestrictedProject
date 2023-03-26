using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class Input_Manager : MonoBehaviour
{
    //input
    private PlayerInput _input;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
    }

    public void SetupLevelInput()
    {
        //setup input
        Dictionary<string, InputAction> playerInputs = GetPlayerInputActions();
        FindObjectOfType<PlayerInitialiser>().InitialisePlayerInput(playerInputs);
        FindObjectOfType<CameraMovement>().SetupInput(playerInputs);
    }

    public Dictionary<string, InputAction> GetUIInputActions()
    {
        Dictionary<string, InputAction> inputDict = new Dictionary<string, InputAction>();

        inputDict.Add("Pause", _input.currentActionMap.FindAction("Pause"));
        inputDict.Add("Move", _input.currentActionMap.FindAction("Move"));
        inputDict.Add("Swap", _input.currentActionMap.FindAction("Swap"));
        inputDict.Add("Select", _input.currentActionMap.FindAction("Select"));
        inputDict.Add("Cancel", _input.currentActionMap.FindAction("Cancel"));

        return inputDict;
    }

    public Dictionary<string, InputAction> GetPlayerInputActions()
    {
        Dictionary<string, InputAction> inputDict = new Dictionary<string, InputAction>();

        inputDict.Add("Move", _input.currentActionMap.FindAction("Move"));
        inputDict.Add("Look", _input.currentActionMap.FindAction("Look"));
        inputDict.Add("Jump", _input.currentActionMap.FindAction("Jump"));
        inputDict.Add("Attack", _input.currentActionMap.FindAction("Attack"));
        inputDict.Add("Target", _input.currentActionMap.FindAction("Target"));
        inputDict.Add("Swap", _input.currentActionMap.FindAction("Swap"));
        inputDict.Add("Draw", _input.currentActionMap.FindAction("Draw"));

        return inputDict;
    }
}
