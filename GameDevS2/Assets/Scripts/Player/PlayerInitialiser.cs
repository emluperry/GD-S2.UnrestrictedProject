using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerInitialiser : MonoBehaviour
{
    public void InitialisePlayerInput(Dictionary<string, InputAction> inputs)
    {
        IInput[] inputComponents = GetComponents<IInput>();
        if(inputComponents.Length > 0)
        {
            foreach(IInput component in inputComponents)
            {
                component.SetupInput(inputs);
            }
        }
    }
}
