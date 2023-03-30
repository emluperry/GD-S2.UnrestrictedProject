using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Scriptable_Decision : ScriptableObject
{
    public abstract bool Decide(State_Manager manager);
}