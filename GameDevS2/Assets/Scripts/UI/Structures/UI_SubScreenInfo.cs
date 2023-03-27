using System;

using UnityEngine;

[Serializable]
public class UI_SubScreenInfo
{
    public UI_OnClickButton button;
    public GameObject screen;

    public Action<UI_SubScreenInfo> onButtonClicked;

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
        onButtonClicked?.Invoke(this);
    }

    public void ActivateScreen()
    {
        button.SetStaysActive(true);
        screen.SetActive(true);
    }

    public void DeactivateScreen()
    {
        button.SetStaysActive(false);
        screen.SetActive(false);
    }
}
