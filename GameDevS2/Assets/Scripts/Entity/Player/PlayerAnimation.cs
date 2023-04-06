using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAnimation : EntityAnimation, IInput
{
    //input actions
    private InputAction _moveInput;
    private InputAction _jumpInput;

    //components
    private PlayerAttack _playerAttack;

    protected override void Awake()
    {
        base.Awake();

        _playerAttack = GetComponent<PlayerAttack>();
        _playerAttack.onCardUsed += Handle_AttackAnimation;
    }

    protected override void OnDestroy()
    {
        DisableInput();

        _playerAttack.onCardUsed += Handle_AttackAnimation;
    }

    #region INPUT SETUP
    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _moveInput = inputs["Move"];
        _jumpInput = inputs["Jump"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_moveInput != null)
        {
            _moveInput.performed += Handle_MovePerformed;
            _jumpInput.performed += Handle_JumpPerformed;
        }
    }

    public void DisableInput()
    {
        if (_moveInput != null)
        {
            _moveInput.performed -= Handle_MovePerformed;
            _jumpInput.performed -= Handle_JumpPerformed;
        }
    }
    #endregion

    #region INPUT

    private void Handle_MovePerformed(InputAction.CallbackContext ctx)
    {
        StartMovement();
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            _animator.SetTrigger("Jump_t");
            _animator.SetBool("Fall_b", false);
            _isGrounded = false;
        }
    }

    private void Handle_AttackAnimation(GDS2_Cards.CARD_TYPE cardType)
    {
        switch (cardType)
        {
            case GDS2_Cards.CARD_TYPE.ATTACK:
                _animator.SetTrigger("Attack_t");
                break;
            case GDS2_Cards.CARD_TYPE.HEALTH:
                _animator.SetTrigger("Magic_t");
                break;
            case GDS2_Cards.CARD_TYPE.DEFENSE:
                _animator.SetTrigger("Shield_t");
                break;
        }

    }

    #endregion
}
