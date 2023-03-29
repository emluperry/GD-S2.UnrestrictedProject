using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

[CreateAssetMenu(fileName = "EvadeAction", menuName = "ScriptableObjects/Enemies/Actions/New Evade Action", order = 1)]
public class EvadeAction : Scriptable_Action
{
    [SerializeField] protected int _maxEvasionDistance = 4;

    public override void Act(State_Manager manager)
    {
        Vector3 direction = Vector3.zero;
        Vector3 differenceVector = Vector3.zero;

        if (Vector3.Distance(manager.transform.position, manager.playerHealthTarget.transform.position) < _maxEvasionDistance)
        {
            //get direction away from player
            differenceVector = (manager.playerHealthTarget.transform.position - manager.transform.position);
            direction = new Vector3(differenceVector.x, 0, differenceVector.z).normalized * -1;
        }
        else
        {
            manager.enemyMovement.StopMovement();
            //strafe? check which directions are possible
            //_moveDirection = _cameraTransform.TransformDirection(_moveInput);
            //_moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z);
        }

        manager.enemyMovement.StartMovement(direction);
    }

    public override void Exit(State_Manager manager)
    {
        manager.enemyMovement.StopMovement();
    }
}
