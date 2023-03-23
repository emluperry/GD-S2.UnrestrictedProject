using System;
using UnityEngine;

using UI_Enums;
using Scene_Enums;

public class UI_Screen_Buttons : MonoBehaviour
{
    [SerializeField] protected UI_ButtonInfo[] _uiButtons;
    [SerializeField] protected Scene_ButtonInfo[] _sceneButtons;

    public Action<UI_SCREENS> onChangeUIScreen;
    public Action<SCENES, int> onChangeScene;

    protected virtual void Awake()
    {
        foreach(UI_ButtonInfo pair in _uiButtons)
        {
            pair.StartListeningForEvents();
            pair.onButtonClicked += HandleUIButton;
        }

        foreach (Scene_ButtonInfo pair in _sceneButtons)
        {
            pair.StartListeningForEvents();
            pair.onButtonClicked += HandleSceneButton;
        }

        if(TryGetComponent(out UI_ScreenAnimation anim))
        {
            anim.onChangeUIScreen += HandleUIButton;
        }
    }

    protected virtual void OnDestroy()
    {
        foreach (UI_ButtonInfo pair in _uiButtons)
        {
            pair.StopListeningForEvents();
            pair.onButtonClicked -= HandleUIButton;
        }

        foreach (Scene_ButtonInfo pair in _sceneButtons)
        {
            pair.StopListeningForEvents();
            pair.onButtonClicked -= HandleSceneButton;
        }

        if (TryGetComponent(out UI_ScreenAnimation anim))
        {
            anim.onChangeUIScreen -= HandleUIButton;
        }
    }

    protected virtual void HandleUIButton(UI_SCREENS screen)
    {
        onChangeUIScreen?.Invoke(screen);
    }

    protected virtual void HandleSceneButton(SCENES scene, int levelNum)
    {
        onChangeScene?.Invoke(scene, levelNum);
    }
}
