using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class UI_OnClickButton : UI_Element, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action onButtonClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        ClickButton();
    }

    public void ClickButton()
    {
        onButtonClicked?.Invoke();

        UpdateButtonColour(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        ActivateButtonSelection();
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        DeactivateButtonSelection();
    }

    public void SetStaysActive(bool enabled)
    {
        _staysActive = enabled;

        UpdateButtonColour(false);
    }
}
