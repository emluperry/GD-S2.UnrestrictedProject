using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    private PlayerInput _input;
    private InputAction _moveInput;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _moveInput = _input.currentActionMap.FindAction("Move");

        _moveInput.performed += Input_MovePerformed();
        _moveInput.canceled += Input_MoveCancelled();
    }

    #region INPUTS


    #endregion


}
