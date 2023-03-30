using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "AttackAction", menuName = "ScriptableObjects/Enemies/Actions/New Attack Action", order = 1)]
public class AttackAction : Scriptable_Action
{
    [SerializeField] protected int _attackDamage = 1;
    [SerializeField] protected float _attackRange = 1;

    public override void Act(State_Manager manager)
    {
        //if player is in range, try to attack!!!
        manager.enemyAttack.StartAttack(_attackDamage, _attackRange);
        Debug.Log("BITES YOU");
    }

    public override void Exit(State_Manager manager)
    {
        return;
    }
}
