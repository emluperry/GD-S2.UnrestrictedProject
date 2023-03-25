using UnityEngine;
using UnityEngine.UI;

public class UI_Slider : Slider
{
    public void ChangeSliderValue(float amount)
    {
        value += amount;

        value = Mathf.Clamp(value, minValue, maxValue);

        onValueChanged?.Invoke(amount);
    }
}