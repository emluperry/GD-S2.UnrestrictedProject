using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_Card : MonoBehaviour
{
    [SerializeField] private Image _cardImageComponent;
    [SerializeField] private TextMeshProUGUI _tmpComponent;
    [SerializeField] private RectTransform _cardSection;

    public void SetImage(Sprite img)
    {
        _cardImageComponent.sprite = img;
    }

    public void SetCardValue(int value)
    {
        _tmpComponent.text = value.ToString();
    }

    public void SetSelected()
    {
        _cardSection.position = new Vector3(_cardSection.position.x, 40, _cardSection.position.z);
    }

    public void Deselect()
    {
        _cardSection.position = new Vector3(_cardSection.position.x, 0, _cardSection.position.z);
    }
}
