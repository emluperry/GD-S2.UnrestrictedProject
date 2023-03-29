using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ChaseAction", menuName = "ScriptableObjects/Enemies/Actions/New Chase Action", order = 1)]
public class ChaseAction : Scriptable_Action
{
    public override void Act(State_Manager manager)
    {
        //get direction
        Vector3 differenceVector = (manager.playerHealthTarget.transform.position - manager.transform.position);
        Vector3 direction = new Vector3(differenceVector.x, 0, differenceVector.z).normalized;

        manager.enemyMovement.StartMovement(direction);
    }

    public override void Exit(State_Manager manager)
    {
        manager.enemyMovement.StopMovement();
    }
}
