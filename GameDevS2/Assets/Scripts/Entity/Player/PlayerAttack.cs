using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : EntityAttack, IInput
{
    //attack input
    protected InputAction _attackInputAction;
    protected bool _isAttackPressed;

    //saved components
    protected PlayerCards _playerCardsComponent;

    protected override void Awake()
    {
        GetComponent<PlayerTargeting>().onTargetChanged += SetTarget;

        _playerCardsComponent = GetComponent<PlayerCards>();
    }

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _attackInputAction = inputs["Attack"];

        _attackInputAction.performed += Input_AttackPerformed;
        _attackInputAction.canceled += Input_AttackCancelled;
    }

    private void OnDestroy()
    {
        if (_attackInputAction != null)
        {
            _attackInputAction.performed -= Input_AttackPerformed;
            _attackInputAction.canceled -= Input_AttackCancelled;
        }
    }

    #region INPUTS

    private void Input_AttackPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused)
            return;

        _isAttackPressed = ctx.ReadValueAsButton();

        if(_attackingCoroutine == null && _isAttackPressed)
        {
            _attackingCoroutine = StartCoroutine(c_AttackingCoroutine());
        }
    }

    private void Input_AttackCancelled(InputAction.CallbackContext ctx)
    {
        _isAttackPressed = false;
    }

    #endregion

    protected override bool GetTargetInRange(out EntityHealth target)
    {
        target = _targetHealth;

        if (target == null)
        {
            return base.GetTargetInRange(out target);
        }

        //push player in range here
        return true;
    }

    protected override void Attack()
    {
        Scriptable_Card currentCard = _playerCardsComponent.UseSelectedCard();

        if (currentCard)
        {
            switch (currentCard.GetCardType())
            {
                case GDS2_Cards.CARD_TYPE.ATTACK:
                    _targetHealth.TakeDamage(currentCard.GetCardPower());
                    break;
                case GDS2_Cards.CARD_TYPE.HEALTH:
                    //try to heal player
                    break;
                case GDS2_Cards.CARD_TYPE.DEFENSE:
                    //add to player defense
                    break;
            }

        }
    }
}
