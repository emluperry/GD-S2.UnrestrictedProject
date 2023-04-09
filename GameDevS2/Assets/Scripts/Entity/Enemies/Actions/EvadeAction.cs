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

        if (Vector3.Distance(manager.transform.position, manager.playerTransform.position) < _maxEvasionDistance)
        {
            //get direction away from player
            differenceVector = (manager.playerTransform.position - manager.transform.position);
            direction = new Vector3(differenceVector.x, 0, differenceVector.z).normalized * -1;

            manager.enemyMovement.StartMovement(direction);
            manager.enemyAnimation.StartMovementAnimation();
            manager.enemySound.PlayMovementSounds();
        }
        else
        {
            manager.enemyMovement.StopMovement();
            manager.enemySound.PlayIdleSounds();
            //strafe? check which directions are possible
            //_moveDirection = _cameraTransform.TransformDirection(_moveInput);
            //_moveDirection = new Vector3(_moveDirection.x, 0, _moveDirection.z); <- code for player strafe
        }
    }

    public override void Exit(State_Manager manager)
    {
        manager.enemyMovement.StopMovement();
    }
}
