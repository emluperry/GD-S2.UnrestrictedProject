using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAttack : MonoBehaviour, IPausable
{
    protected Coroutine _attackingCoroutine;

    [Header("Attacking")]
    [SerializeField][Min(0f)] protected float _maxAttackDelay = 0.5f;
    protected float _currentAttackDelay = 0;

    [SerializeField][Min(0f)] protected float _attackRange = 2f;

    [SerializeField] protected Transform _attackPoint;

    [SerializeField] protected LayerMask _layersToIgnore;

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

        if(!GetTargetInRange(out EntityHealth target)) //if the entity cannot get a target in range, quit routine
        {
            _currentAttackDelay = 0;
            _attackingCoroutine = null;
            yield break;
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

    protected virtual bool GetTargetInRange(out EntityHealth target)
    {
        Ray ray = new Ray(_attackPoint.position, transform.forward);
        Debug.DrawRay(_attackPoint.position, transform.forward * _attackRange);

        if (!Physics.SphereCast(ray, 2, out RaycastHit hitInfo, _attackRange, _layersToIgnore))
        {
            Debug.Log("Didn't hit anything - returning early.");
            _attackingCoroutine = null;

            target = null;
            return false;
        }

        if (hitInfo.transform.gameObject.TryGetComponent(out EntityHealth health))
        {
            Debug.Log(hitInfo.transform.name);
            target = health;
            return true;
        }
        else
        {
            target = null;
            return false;
        }
    }

    protected virtual void Attack()
    {

    }

    public LayerMask GetIgnoredLayers()
    {
        return _layersToIgnore;
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
