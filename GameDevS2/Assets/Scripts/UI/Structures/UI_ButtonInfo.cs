using System;

using UI_Enums;

[Serializable]
public class UI_ButtonInfo
{
    public UI_OnClickButton button;
    public UI_SCREENS screen;

    public Action<UI_SCREENS> onButtonClicked;

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
        onButtonClicked?.Invoke(screen);
    }
}
