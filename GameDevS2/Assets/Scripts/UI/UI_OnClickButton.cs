using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using TMPro;

public class UI_OnClickButton : MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    enum BUTTON_TYPE
    {
        TEXT,
        ICON
    }

    private bool _staysActive = false;

    private Color _imageColour = Color.white;
    [SerializeField] private Color _imageHoverColour = Color.white;
    
    private Color _textColour = Color.white;
    [SerializeField] private Color _textHoverColour = Color.black;

    private Image _imageRenderer;
    [SerializeField] private BUTTON_TYPE _buttonType = BUTTON_TYPE.TEXT;
    private TextMeshProUGUI _textRenderer;
    private Image _iconRenderer;

    public Action onButtonClicked;

    private void Awake()
    {
        Image[] images = GetComponentsInChildren<Image>();
        _imageRenderer = images[0];
        _imageColour = _imageRenderer.color;

        if(_buttonType == BUTTON_TYPE.TEXT)
        {
            _textRenderer = GetComponentInChildren<TextMeshProUGUI>();
            _textColour = _textRenderer.color;
        }
        else if(_buttonType == BUTTON_TYPE.ICON && images.Length > 1)
        {
            _iconRenderer = images[1];
            _textColour = _iconRenderer.color;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        onButtonClicked?.Invoke();

        UpdateButtonColour(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        UpdateButtonColour(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        UpdateButtonColour(false);
    }

    public void SetStaysActive(bool enabled)
    {
        _staysActive = enabled;

        UpdateButtonColour(false);
    }

    private void UpdateButtonColour(bool isActive)
    {
        if (_staysActive)
            isActive = true;

        if (_imageRenderer)
        {
            if(isActive)
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

        if (_iconRenderer)
        {
            if (isActive)
            {
                _iconRenderer.color = _textHoverColour;
            }
            else
            {
                _iconRenderer.color = _textColour;
            }
        }
    }
}
