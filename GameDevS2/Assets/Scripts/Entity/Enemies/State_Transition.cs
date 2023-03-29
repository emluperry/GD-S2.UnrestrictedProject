using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class State_Transition
{
    public Scriptable_Decision decision;
    public Scriptable_State trueState;
    public Scriptable_State falseState;

    public Scriptable_State Decide(State_Manager manager)
    {
        bool decisionMade = decision.Decide(manager);

        if (decisionMade)
            return trueState;
        else
            return falseState;
    }
}