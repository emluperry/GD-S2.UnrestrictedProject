using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : MonoBehaviour, IPausable
{
    protected Coroutine _attackingCoroutine;

    [Header("Attacking")]
    [SerializeField][Min(0f)] protected float _maxAttackDelay = 0.5f;
    protected float _currentAttackDelay = 0;
    [SerializeField][Min(0f)] protected float _attackRaycastRange = 2f;
    [SerializeField] protected LayerMask _attackableLayers;

    //targeted component
    protected EntityHealth _targetHealth;

    protected bool _isPaused = false;

    protected virtual void Awake()
    {
        //get targeting component?
    }

    protected IEnumerator c_AttackingCoroutine()
    {
        yield return new WaitForFixedUpdate();

        EntityHealth target = _targetHealth;

        if (target == null)
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
                _attackingCoroutine = null;
                yield break;
            }
        }

        Attack();

        while (_currentAttackDelay < _maxAttackDelay)
        {
            _currentAttackDelay += Time.fixedDeltaTime;
            yield return new WaitForFixedUpdate();
            yield return new WaitUntil(() => !_isPaused);
        }

        _currentAttackDelay = 0;
        _attackingCoroutine = null;
    }

    protected virtual void Attack()
    {
        //ex: card logic here
        //Scriptable_Card currentCard = _playerCardsComponent.UseSelectedCard();

        //if (currentCard)
        //{
        //    switch (currentCard.GetCardType())
        //    {
        //        case GDS2_Cards.CARD_TYPE.ATTACK:
        //            _targetHealth.TakeDamage(currentCard.GetCardPower());
        //            break;
        //        case GDS2_Cards.CARD_TYPE.HEALTH:
        //            //try to heal player
        //            break;
        //        case GDS2_Cards.CARD_TYPE.DEFENSE:
        //            //add to player defense
        //            break;
        //    }

        //}
    }

    protected void SetTarget(GameObject target)
    {
        if (target == null)
            _targetHealth = null;
        else
            _targetHealth = target.GetComponent<EntityHealth>();
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }
}
