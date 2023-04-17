using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IdleAction", menuName = "ScriptableObjects/Enemies/Actions/New Idle Action", order = 1)]
public class IdleAction : Scriptable_Action
{
    public override void Act(State_Manager manager)
    {
        //just stand there.
        manager.enemySound.PlayIdleSounds();
    }

    public override void Exit(State_Manager manager)
    {
        return;
    }
}
