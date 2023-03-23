using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "UI_Screen_Database", menuName = "ScriptableObjects/Enum Pairs/UI_Screen_Enum", order = 2)]
public class ScriptableObj_UIDatabase : ScriptableObject
{
    public UI_ScreenInfo[] UIPairs;
}
