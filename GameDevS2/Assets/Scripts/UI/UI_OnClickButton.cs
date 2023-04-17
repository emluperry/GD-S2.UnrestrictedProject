using System;
using UnityEngine.EventSystems;
using TMPro;

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

    public void UpdateText(string newText)
    {
        TextMeshProUGUI textComp = _textRenderer.GetComponent<TextMeshProUGUI>();
        if (textComp)
        {
            textComp.text = newText;
        }
    }
}
