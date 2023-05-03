using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : EntityAttack, IInput
{
    //attack input
    protected InputAction _attackInputAction;
    protected bool _isAttackPressed;

    [SerializeField] private float _maxAttackForce = 5;
    [SerializeField] private float _verticalComponent = 0.3f;

    //saved components
    protected PlayerCards _playerCardsComponent;
    protected EntityHealth _playerHealth;
    protected Rigidbody _rb;

    public Action<GDS2_Cards.CARD_TYPE> onCardUsed;

    protected override void Awake()
    {
        GetComponent<PlayerTargeting>().onTargetChanged += SetTarget;

        _playerCardsComponent = GetComponent<PlayerCards>();
        _playerHealth = GetComponent<EntityHealth>();
        _rb = GetComponent<Rigidbody>();
    }

    private void OnDestroy()
    {
        DisableInput();
    }

    #region INPUT SETUP

    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _attackInputAction = inputs["Attack"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_attackInputAction != null)
        {
            _attackInputAction.performed += Input_AttackPerformed;
            _attackInputAction.canceled += Input_AttackCancelled;
        }
    }

    public void DisableInput()
    {
        if (_attackInputAction != null)
        {
            _attackInputAction.performed -= Input_AttackPerformed;
            _attackInputAction.canceled -= Input_AttackCancelled;
        }
    }

    #endregion

    #region INPUTS

    private void Input_AttackPerformed(InputAction.CallbackContext ctx)
    {
        if (_isPaused || !_playerCardsComponent.isInCombat)
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

        PushPlayerInRange(target);

        return true;
    }

    protected override void Attack(EntityHealth target)
    {
        Scriptable_Card currentCard = _playerCardsComponent.UseSelectedCard();

        if (currentCard)
        {
            int cardPower = currentCard.GetCardPower();
            GDS2_Cards.CARD_TYPE cardType = currentCard.GetCardType();

            switch (cardType)
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

            onCardUsed?.Invoke(cardType);
        }
    }

    protected void PushPlayerInRange(EntityHealth target)
    {
        //push player in range here - get direction between target and player give slight vertical component, multiply by force, apply to rb
        Vector3 direction = target.transform.position - transform.position;
        direction.Normalize();

        _rb.AddForce(new Vector3(direction.x, direction.y + _verticalComponent, direction.z) * _maxAttackForce, ForceMode.Impulse);
    }
}
