using System;
using UnityEngine.SceneManagement;

using Scene_Enums;

[Serializable]
public class Scene_SceneInfo
{
    public SCENES sEnum;
    public int levelNum = -1;
    public string sceneName = "name";
    public bool isPausable = false;
    public bool isBossLevel = false;
}
