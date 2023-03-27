using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

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

    public void InitialisePauseEvents(ref Action<bool> onLoadPause)
    {
        IPausable[] pausableComponents = GetComponents<IPausable>();
        if (pausableComponents.Length > 0)
        {
            foreach (IPausable component in pausableComponents)
            {
                onLoadPause += component.PauseGame;
            }
        }
    }

    public void StopListeningForPause(ref Action<bool> onLoadPause)
    {
        IPausable[] pausableComponents = GetComponents<IPausable>();
        if (pausableComponents.Length > 0)
        {
            foreach (IPausable component in pausableComponents)
            {
                onLoadPause -= component.PauseGame;
            }
        }
    }
}
