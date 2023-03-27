using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Scene_Enums;

[CreateAssetMenu(fileName = "Scene_Database", menuName = "ScriptableObjects/Enum Pairs/Scene_Database", order = 1)]
public class ScriptableObj_SceneDatabase : ScriptableObject
{
    [SerializeField] private Scene_SceneInfo[] scenePairs;

    public string GetSceneName(SCENES type)
    {
        foreach(Scene_SceneInfo pair in scenePairs)
        {
            if(pair.sEnum == type)
            {
                return pair.sceneName;
            }
        }

        Debug.Log("Invalid scene type: returning main menu.");
        return GetSceneName(SCENES.START);
    }

    public string GetLevelName(int levelNum)
    {
        foreach (Scene_SceneInfo pair in scenePairs)
        {
            if (pair.sEnum == SCENES.LEVEL && pair.levelNum == levelNum)
            {
                return pair.sceneName;
            }
        }

        Debug.Log("Invalid level number: returning main menu.");
        return GetSceneName(SCENES.START);
    }

    public bool IsScenePausable(SCENES type, int levelNum = -1)
    {
        bool searchLevels = levelNum == -1 ? false : true;
        foreach (Scene_SceneInfo pair in scenePairs)
        {
            if (pair.sEnum == type)
            {
                if(searchLevels)
                {
                    if(pair.levelNum == levelNum)
                        return pair.isPausable;
                }
                else
                {
                    return pair.isPausable;
                }
                
            }
        }

        Debug.Log("Invalid scene type: returning false.");
        return false;
    }
}
