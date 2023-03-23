using System;

using Scene_Enums;

[Serializable]
public class Scene_ButtonInfo
{
    public UI_OnClickButton button;
    public SCENES scene;
    public int levelNum;

    public Action<SCENES, int> onButtonClicked;

    public void StartListeningForEvents()
    {
        button.onButtonClicked += ButtonClicked;
    }

    public void StopListeningForEvents()
    {
        button.onButtonClicked -= ButtonClicked;
    }

    private void ButtonClicked()
    {
        onButtonClicked?.Invoke(scene, levelNum);
    }
}
