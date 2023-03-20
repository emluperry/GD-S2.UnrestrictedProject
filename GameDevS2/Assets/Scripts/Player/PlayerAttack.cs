using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerAttack : MonoBehaviour
{
    //input
    private PlayerInput _input;

    //attack input
    private InputAction _attackInputAction;
    private bool _isAttackPressed;

    private Coroutine _attackingCoroutine;

    [Header("Attacking")]
    [SerializeField][Min(0f)] private float _maxAttackDelay = 0.5f;
    private float _currentAttackDelay = 0;
    [SerializeField][Min(0f)] private float _attackRaycastRange = 2f;
    [SerializeField] private LayerMask _attackableLayers;

    //targeted component
    private EntityHealth _targetHealth;

    //saved components
    private PlayerCards _playerCardsComponent;

    private void Awake()
    {
        _input = GetComponent<PlayerInput>();
        _attackInputAction = _input.currentActionMap.FindAction("Attack");

        _attackInputAction.performed += Input_AttackPerformed;
        _attackInputAction.canceled += Input_AttackCancelled;

        GetComponent<PlayerTargeting>().onTargetChanged += SetTarget;

        _playerCardsComponent = GetComponent<PlayerCards>();
    }

    #region INPUTS

    private void Input_AttackPerformed(InputAction.CallbackContext ctx)
    {
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

    private IEnumerator c_AttackingCoroutine()
    {
        yield return new WaitForFixedUpdate();

        EntityHealth target = _targetHealth;

        if(target == null)
        {
            //forward raycast
            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, _attackRaycastRange, _attackableLayers, QueryTriggerInteraction.Ignore))
            {
                if (hitInfo.transform.gameObject.TryGetComponent(out EntityHealth health))
                    target = health;
            }
            else
            {
                Debug.Log("Didn't hit anything - returning early.");
                yield break;
            }
        }

        //card logic here
        Scriptable_Card currentCard = _playerCardsComponent.UseSelectedCard();

        if (currentCard)
        {
            Debug.Log(currentCard.GetCardPower());
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

        while (_currentAttackDelay < _maxAttackDelay)
        {
            _currentAttackDelay += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
        }

        _currentAttackDelay = 0;
        _attackingCoroutine = null;
    }

    private void SetTarget(GameObject target)
    {
        if (target == null)
            _targetHealth = null;
        else
            _targetHealth = target.GetComponent<EntityHealth>();
    }
}
