using UnityEngine;
using UnityEngine.UI;
using TMPro;

using UI_Enums;
using System.Collections.Generic;

public class UI_Element : MonoBehaviour
{
    protected enum BUTTON_TYPE
    {
        TEXT,
        ICON
    }

    public UI_ELEMENT_TYPE type { protected set; get; } = UI_ELEMENT_TYPE.BUTTON;

    protected bool _staysActive = false;

    protected Color _imageColour = Color.white;
    [SerializeField] protected Color _imageHoverColour = Color.white;

    protected Color _textColour = Color.white;
    [SerializeField] protected Color _textHoverColour = Color.black;

    protected Image _imageRenderer;
    protected MaskableGraphic _textRenderer;

    protected virtual void Awake()
    {
        if(TryGetComponent(out UI_Slider slider))
        {
            type = UI_ELEMENT_TYPE.SLIDER;
        }

        _imageRenderer = GetComponentInChildren<Image>();
        _imageColour = _imageRenderer.color;

        List<Transform> transforms = new List<Transform>(GetComponentsInChildren<Transform>());
        transforms.Remove(transform);
        foreach(Transform tf in transforms)
        {
            if(tf.TryGetComponent(out MaskableGraphic graphic))
            {
                _textRenderer = graphic;
                _textColour = _textRenderer.color;
                break;
            }
        }
    }

    public void ActivateButtonSelection()
    {
        UpdateButtonColour(true);
    }

    public void DeactivateButtonSelection()
    {
        UpdateButtonColour(false);
    }

    protected void UpdateButtonColour(bool isActive)
    {
        if (_staysActive)
            isActive = true;

        if (_imageRenderer)
        {
            if (isActive)
            {
                _imageRenderer.color = _imageHoverColour;
            }
            else
            {
                _imageRenderer.color = _imageColour;
            }
        }

        if (_textRenderer)
        {
            if (isActive)
            {
                _textRenderer.color = _textHoverColour;
            }
            else
            {
                _textRenderer.color = _textColour;
            }
        }
    }
}