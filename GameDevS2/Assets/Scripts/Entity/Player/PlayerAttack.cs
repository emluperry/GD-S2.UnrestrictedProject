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
    protected EntityHealth _playerHealth;

    protected override void Awake()
    {
        GetComponent<PlayerTargeting>().onTargetChanged += SetTarget;

        _playerCardsComponent = GetComponent<PlayerCards>();
        _playerHealth = GetComponent<EntityHealth>();
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
        Scriptable_Card currentCard = _playerCardsComponent.GetSelectedCard();

        if(currentCard.GetCardType() != GDS2_Cards.CARD_TYPE.ATTACK)
        {
            //card is used on player, so target not needed!
            target = null;
            return true;
        }

        target = _targetHealth;

        if (target == null)
        {
            return base.GetTargetInRange(out target);
        }

        //push player in range here
        return true;
    }

    protected override void Attack(EntityHealth target)
    {
        Scriptable_Card currentCard = _playerCardsComponent.UseSelectedCard();
        int cardPower = currentCard.GetCardPower();

        if (currentCard)
        {
            switch (currentCard.GetCardType())
            {
                case GDS2_Cards.CARD_TYPE.ATTACK:
                    if(target != null)
                        target.TakeDamage(cardPower);
                    break;
                case GDS2_Cards.CARD_TYPE.HEALTH:
                    _playerHealth.HealHealth(cardPower);
                    break;
                case GDS2_Cards.CARD_TYPE.DEFENSE:
                    _playerHealth.IncreaseShield(cardPower);
                    break;
            }

        }
    }
}
