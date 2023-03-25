using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : UI_Element
{
    protected Slider _sliderComponent;

    [Header("Increment Amount")]
    [SerializeField] private float _incrementAmount = 1;

    protected override void Awake()
    {
        type = UI_Enums.UI_ELEMENT_TYPE.SLIDER;

        _sliderComponent = GetComponent<Slider>();

        base.Awake();
    }

    public void IncrementSliderValue(int direction)
    {
        float val = _incrementAmount * direction;
        _sliderComponent.value += val;

        _sliderComponent.value = Mathf.Clamp(_sliderComponent.value, _sliderComponent.minValue, _sliderComponent.maxValue);

        _sliderComponent.onValueChanged?.Invoke(val);
    }
}