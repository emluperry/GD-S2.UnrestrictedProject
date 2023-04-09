using System;
using UnityEngine;

using UI_Enums;
using Scene_Enums;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class UI_Screen : MonoBehaviour
{
    [Header("Audio")]
    [SerializeField] protected AudioClip _selectClip;
    protected AudioSource _source;

    [Header("Buttons")]
    [SerializeField] public UI_SCREENS screenType = UI_SCREENS.NONE;
    [SerializeField] protected UI_ButtonInfo[] _uiButtons;
    [SerializeField] protected Scene_ButtonInfo[] _sceneButtons;

    protected UI_Navigation _navigationComponent;

    public Action<UI_SCREENS> onChangeUIScreen;
    public Action<SCENES, int> onChangeScene;

    protected virtual void Awake()
    {
        _source = GetComponent<AudioSource>();

        foreach(UI_ButtonInfo pair in _uiButtons)
        {
            pair.button.SetupElement();
            pair.StartListeningForEvents();
            pair.onButtonClicked += HandleUIButton;
        }

        foreach (Scene_ButtonInfo pair in _sceneButtons)
        {
            pair.button.SetupElement();
            pair.StartListeningForEvents();
            pair.onButtonClicked += HandleSceneButton;
        }

        if(TryGetComponent(out UI_ScreenAnimation anim))
        {
            anim.onChangeUIScreen += HandleUIButton;
        }

        if(TryGetComponent(out UI_Navigation navi))
        {
            _navigationComponent = navi;
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

    public virtual void SetupInput(Dictionary<string, InputAction> inputs)
    {
        if(_navigationComponent != null)
        {
            _navigationComponent.SetupInput(inputs);
        }
    }

    public virtual void ActivateInput()
    {
        if (_navigationComponent != null)
        {
            _navigationComponent.EnableInput();
        }
    }

    public virtual void DeactivateInput()
    {
        if(_navigationComponent != null)
        {
            _navigationComponent.DisableInput();
        }
    }

    protected virtual void HandleUIButton(UI_SCREENS screen)
    {
        PlaySelectedSound();
        onChangeUIScreen?.Invoke(screen);
    }

    protected virtual void HandleSceneButton(SCENES scene, int levelNum)
    {
        PlaySelectedSound();
        onChangeScene?.Invoke(scene, levelNum);
    }

    protected void PlaySelectedSound()
    {
        if(_source)
        {
            if (_source.clip != _selectClip)
                _source.clip = _selectClip;

            _source.Play();
        }
    }
}
