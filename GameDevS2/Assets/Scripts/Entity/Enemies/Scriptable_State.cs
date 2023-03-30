using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;

[CreateAssetMenu(fileName = "NewState", menuName = "ScriptableObjects/Enemies/New State", order = 1)]
public class Scriptable_State : ScriptableObject
{
    public Scriptable_Action[] actions;
    public State_Transition[] transitions;

    public void UpdateState(State_Manager manager)
    {
        DoActions(manager);
        CheckTransitions(manager);
    }

    private void DoActions(State_Manager manager)
    {
        foreach(Scriptable_Action action in actions)
        {
            action.Act(manager);
        }
    }

    private void CheckTransitions(State_Manager manager)
    {
        foreach(State_Transition transition in transitions)
        {
            Scriptable_State newState = transition.Decide(manager);

            if (newState)
            {
                manager.TransitionToState(newState);
                break;
            }
        }
    }

    public void ExitState(State_Manager manager)
    {
        foreach(Scriptable_Action action in actions)
        {
            action.Exit(manager);
        }
    }
}
