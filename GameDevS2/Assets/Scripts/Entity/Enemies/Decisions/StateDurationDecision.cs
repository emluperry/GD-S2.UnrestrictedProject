using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DurationDecision", menuName = "ScriptableObjects/Enemies/Decisions/New State Duration Decision", order = 1)]
public class StateDurationDecision : Scriptable_Decision
{
    [SerializeField] private float _stateDuration = 0.5f;
    public override bool Decide(State_Manager manager)
    {
        return manager.IncrementElapsedTime(_stateDuration);
    }
}