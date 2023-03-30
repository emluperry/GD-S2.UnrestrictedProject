using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : EntityAttack
{
    private int _attackDamage = 0;

    public void StartAttack(int attackDamage, float attackRange)
    {
        if (_attackingCoroutine == null)
        {
            _attackDamage = attackDamage;
            _attackRange = attackRange;

            _attackingCoroutine = StartCoroutine(c_AttackingCoroutine());
        }
    }

    protected override void Attack(EntityHealth target)
    {
        if (target)
            target.TakeDamage(_attackDamage);
    }
}
