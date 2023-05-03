using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;
using UnityEngine.UI;

[Serializable]
public class UI_Tutorial_Input_Pair
{
    public Image TutorialImage;
    public string TutorialInputName = "NONE";
    public InputAction TutorialInputAction { get; private set; }

    public void SetInputAction(InputAction action)
    {
        TutorialInputAction = action;
    }
}
