using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InRangeDecision", menuName = "ScriptableObjects/Enemies/Decisions/New In-Range Decision", order = 1)]
public class InRangeDecision : Scriptable_Decision
{
    [SerializeField] private float _attackRange = 1;
    public override bool Decide(State_Manager manager)
    {
        if (Vector3.Distance(manager.transform.position, manager.playerTransform.transform.position) <= _attackRange)
            return true;

        return false;
    }
}