using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InRangeDecision", menuName = "ScriptableObjects/Enemies/Decisions/New In-Range Decision", order = 1)]
public class InRangeDecision : Scriptable_Decision
{
    [SerializeField] private float _attackRange = 1; 

    public override bool Decide(State_Manager manager)
    {
        if (Physics.Raycast(manager.transform.position, manager.transform.forward, out RaycastHit hitInfo, _attackRange, manager.enemyAttack.GetIgnoredLayers().value, QueryTriggerInteraction.Ignore))
        {
            if (hitInfo.transform.gameObject.TryGetComponent(out EntityHealth health))
                return true;
        }

        return false;
    }
}