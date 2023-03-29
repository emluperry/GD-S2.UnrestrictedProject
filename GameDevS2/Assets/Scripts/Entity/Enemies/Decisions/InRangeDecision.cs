using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InRangeDecision", menuName = "ScriptableObjects/Enemies/Decisions/New In-Range Decision", order = 1)]
public class InRangeDecision : Scriptable_Decision
{
    public override bool Decide(State_Manager manager)
    {
        if(manager.enemyAttack.GetTargetInRange(out EntityHealth health))
            return true;

        return false;
    }
}