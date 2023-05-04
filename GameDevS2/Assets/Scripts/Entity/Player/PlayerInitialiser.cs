using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Windows;

public class PlayerInitialiser : MonoBehaviour
{
    public Dictionary<string, InputAction> savedInputs { private set; get; }
    public PlayerMovement movement { private set; get; }
    public EntityHealth health { private set; get; }
    public PlayerCards cards { private set; get; }
    public PlayerTargeting targeting { private set; get; }

    public EntityAnimation entityAnimation { private set; get; }

    public Rigidbody entityRb { private set; get; }

    private bool _isGrounded = true;

    private void Awake()
    {
        health = GetComponent<EntityHealth>();
        cards = GetComponent<PlayerCards>();
        targeting = GetComponent<PlayerTargeting>();
        movement = GetComponent<PlayerMovement>();
        entityAnimation = GetComponent<EntityAnimation>();
        entityRb = GetComponent<Rigidbody>();

        entityAnimation.SetupValues(movement.GetMaxSpeed());

        //events
        health.onDead += PlayerKilled;
        health.onDead += entityAnimation.OnDead;
    }

    private void OnDestroy()
    {
        health.onDead -= PlayerKilled;
        health.onDead -= entityAnimation.OnDead;
    }

    protected void FixedUpdate()
    {
        bool touchingGround = Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f);

        if(touchingGround != _isGrounded)
        {
            _isGrounded = touchingGround;
            UpdateGrounded(touchingGround);
        }
    }

    public bool GetIsGrounded()
    {
        return _isGrounded;
    }

    public void UpdateGrounded(bool isGrounded)
    {
        IGroundable[] groundedComponents = GetComponents<IGroundable>();
        if (groundedComponents.Length > 0)
        {
            foreach (IGroundable component in groundedComponents)
            {
                component.UpdateGrounded(isGrounded);
            }
        }
    }

    #region INPUT
    public void InitialisePlayerInput(Dictionary<string, InputAction> inputs)
    {
        savedInputs = inputs;

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
    #endregion

    #region PAUSE
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
    #endregion

    private void PlayerKilled(EntityHealth playerHealth)
    {
        StopListeningForPlayerInput();

        movement.OnDead();
    }
}
