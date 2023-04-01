using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInitialiser : MonoBehaviour
{
    public PlayerMovement movement { private set; get; }
    public EntityHealth health { private set; get; }
    public PlayerCards cards { private set; get; }
    public PlayerTargeting targeting { private set; get; }

    public EntityAnimation entityAnimation { private set; get; }

    private void Awake()
    {
        health = GetComponent<EntityHealth>();
        cards = GetComponent<PlayerCards>();
        targeting = GetComponent<PlayerTargeting>();
        movement = GetComponent<PlayerMovement>();
        entityAnimation = GetComponent<EntityAnimation>();

        health.onDead += PlayerKilled;
    }

    private void OnDestroy()
    {
        health.onDead -= PlayerKilled;
    }

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

    public void StartListeningForPlayerInput()
    {
        IInput[] inputComponents = GetComponents<IInput>();
        if (inputComponents.Length > 0)
        {
            foreach (IInput component in inputComponents)
            {
                component.EnableInput();
            }
        }
    }

    public void StopListeningForPlayerInput()
    {
        IInput[] inputComponents = GetComponents<IInput>();
        if (inputComponents.Length > 0)
        {
            foreach (IInput component in inputComponents)
            {
                component.DisableInput();
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

    private void PlayerKilled(EntityHealth playerHealth)
    {
        StopListeningForPlayerInput();
    }
}
