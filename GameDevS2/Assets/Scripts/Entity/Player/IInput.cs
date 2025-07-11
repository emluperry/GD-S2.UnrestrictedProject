﻿using System.Collections.Generic;
using UnityEngine.InputSystem;

public interface IInput
{
    public void SetupInput(Dictionary<string, InputAction> inputs);

    public void EnableInput();

    public void DisableInput();
}
