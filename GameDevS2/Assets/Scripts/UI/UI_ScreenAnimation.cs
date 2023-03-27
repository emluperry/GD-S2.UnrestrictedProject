using System;
using System.Collections;
using System.Collections.Generic;
using UI_Enums;
using UnityEngine;

public class UI_ScreenAnimation : MonoBehaviour
{
    [SerializeField] protected UI_ButtonInfo[] _uiButtons;
    [SerializeField] private Animator _animatedComponent;

    public Action<UI_SCREENS> onChangeUIScreen;

    private void Awake()
    {
        foreach (UI_ButtonInfo pair in _uiButtons)
        {
            pair.StartListeningForEvents();
            pair.onButtonClicked += HandleUIAnimation;
        }

        if (_animatedComponent == null)
            TryGetComponent(out _animatedComponent);
    }

    private void OnDestroy()
    {
        foreach (UI_ButtonInfo pair in _uiButtons)
        {
            pair.StopListeningForEvents();
            pair.onButtonClicked -= HandleUIAnimation;
        }
    }

    public virtual void HandleUIAnimation(UI_SCREENS screen)
    {
        StartCoroutine(StartTransitionAnimation(screen));
    }

    private IEnumerator StartTransitionAnimation(UI_SCREENS screen)
    {
        _animatedComponent.Play("on" + screen.ToString());
        yield return new WaitForEndOfFrame();
        yield return new WaitForSeconds(_animatedComponent.GetCurrentAnimatorStateInfo(0).length);
        onChangeUIScreen?.Invoke(screen);
    }
}
