using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerSound : EntitySound, IInput, IGroundable
{
    protected PlayerAttack _playerAttack;

    //input actions
    protected InputAction _moveInput;
    protected InputAction _jumpInput;
    protected InputAction _drawInput;

    protected bool _isGrounded = true;

    //extra sound
    [Header("Additional Sounds")]
    [SerializeField] protected AudioClip[] _magic;
    [SerializeField] protected AudioClip[] _shield;
    [SerializeField] protected AudioClip[] _cardShuffle;

    protected override void Awake()
    {
        base.Awake();

        _playerAttack = GetComponent<PlayerAttack>();
        _playerAttack.onCardUsed += Handle_AttackSound;
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        _playerAttack.onCardUsed -= Handle_AttackSound;
    }

    #region INPUT SETUP
    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _moveInput = inputs["Move"];
        _jumpInput = inputs["Jump"];
        _drawInput = inputs["Draw"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_moveInput != null)
        {
            _moveInput.performed += Handle_MovePerformed;
            _moveInput.canceled += Handle_MoveCancelled;
            _jumpInput.performed += Handle_JumpPerformed;
            _drawInput.performed += Handle_DrawPerformed;
        }
    }

    public void DisableInput()
    {
        if (_moveInput != null)
        {
            _moveInput.performed -= Handle_MovePerformed;
            _moveInput.canceled -= Handle_MoveCancelled;
            _jumpInput.performed -= Handle_JumpPerformed;
            _drawInput.performed -= Handle_DrawPerformed;
        }
    }
    #endregion

    #region INPUT

    private void Handle_MovePerformed(InputAction.CallbackContext ctx)
    {
        StopRepeatedSounds();
        PlayMove();
    }

    private void Handle_MoveCancelled(InputAction.CallbackContext ctx)
    {
        StopRepeatedSounds();
        PlayIdle();
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext ctx)
    {
        if (_isGrounded)
        {
            PlayJump();
            _isGrounded = false;
        }
    }

    private void Handle_DrawPerformed(InputAction.CallbackContext ctx)
    {
        PlayDraw();
    }

    private void Handle_AttackSound(GDS2_Cards.CARD_TYPE cardType)
    {
        switch (cardType)
        {
            case GDS2_Cards.CARD_TYPE.ATTACK:
                PlayAttack();
                break;
            case GDS2_Cards.CARD_TYPE.HEALTH:
                PlayMagic();
                break;
            case GDS2_Cards.CARD_TYPE.DEFENSE:
                PlayShield();
                break;
        }

    }
    #endregion

    public void UpdateGrounded(bool isGrounded)
    {
        _isGrounded = isGrounded;

        if(isGrounded)
        {
            PlayLand();
        }
    }

    protected void PlayMagic()
    {
        PlaySound(_magic);
    }

    protected void PlayShield()
    {
        PlaySound(_shield);
    }

    protected void PlayDraw()
    {
        PlaySound(_cardShuffle);
    }
}
