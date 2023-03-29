using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scriptable_Action : ScriptableObject
{
    public abstract void Act(State_Manager manager);

    public abstract void Exit(State_Manager manager);
}
