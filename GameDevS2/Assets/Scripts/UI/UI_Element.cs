using UnityEngine;
using UnityEngine.UI;

using UI_Enums;
using System.Collections.Generic;
using Unity.VisualScripting;

public class UI_Element : MonoBehaviour
{
    protected enum BUTTON_TYPE
    {
        TEXT,
        ICON
    }

    public UI_ELEMENT_TYPE type { protected set; get; } = UI_ELEMENT_TYPE.BUTTON;

    public bool _staysActive { protected set; get; } = false;
    protected bool _selectedElement = false;

    [Header("Colours")]
    protected Color _imageColour = Color.white;
    [SerializeField] protected Color _imageHoverColour = Color.white;

    protected Color _textColour = Color.white;
    [SerializeField] protected Color _textHoverColour = Color.black;

    protected Color _selectableElementColour = Color.white;
    [SerializeField] protected Color _elementSelectedColour = Color.black;

    protected Image _imageRenderer;
    protected MaskableGraphic _textRenderer;
    [SerializeField] MaskableGraphic _selectedElementRenderer;

    protected virtual void Awake()
    {
        if(TryGetComponent(out UI_Slider slider))
        {
            type = UI_ELEMENT_TYPE.SLIDER;
        }

        _imageRenderer = GetComponentInChildren<Image>();
        _imageColour = _imageRenderer.color;

        if (_selectedElementRenderer)
            _selectableElementColour = _selectedElementRenderer.color;

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

    public virtual void SelectElement()
    {
        _selectedElement = true;
        UpdateButtonColour(true);
    }

    public virtual void DeselectElement()
    {
        _selectedElement = false;
        UpdateButtonColour(false);
    }

    public virtual void ActivateButtonSelection()
    {
        UpdateButtonColour(true);
    }

    public virtual void DeactivateButtonSelection()
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

        if (_selectedElementRenderer && _selectedElement)
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