using System;
using UnityEngine.EventSystems;

public class UI_OnClickButton : UI_Element, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Action onButtonClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        SelectElement();
    }

    public override void SelectElement()
    {
        onButtonClicked?.Invoke();

        base.DeselectElement();
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
