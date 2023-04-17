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

    protected bool _staysActive = false;
    public bool selectedElement { protected set; get; } = false;

    [Header("Colours")]
    protected Color _imageColour = Color.white;
    [SerializeField] protected Color _imageHoverColour = Color.white;

    protected Color _textColour = Color.white;
    [SerializeField] protected Color _textHoverColour = Color.black;

    protected Color _selectableElementColour = Color.white;
    [SerializeField] protected Color _elementSelectedColour = Color.black;

    [SerializeField] protected MaskableGraphic _imageRenderer;
    [SerializeField] protected MaskableGraphic _textRenderer;
    [SerializeField] MaskableGraphic _selectedElementRenderer;

    protected virtual void Awake()
    {
        SetupElement();
    }

    public virtual void SetupElement()
    {
        if (_imageRenderer)
            _imageColour = _imageRenderer.color;

        if (_textRenderer)
            _textColour = _textRenderer.color;

        if (_selectedElementRenderer)
            _selectableElementColour = _selectedElementRenderer.color;
    }

    public virtual void SelectElement()
    {
        selectedElement = true;
        UpdateButtonColour(true);
    }

    public virtual void DeselectElement()
    {
        selectedElement = false;
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

        if (_selectedElementRenderer)
        {
            if (isActive && selectedElement)
            {
                _selectedElementRenderer.color = _elementSelectedColour;
            }
            else
            {
                _selectedElementRenderer.color = _selectableElementColour;
            }
        }
    }
}