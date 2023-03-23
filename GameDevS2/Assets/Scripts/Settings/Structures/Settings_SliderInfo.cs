using System;
using UnityEngine.UI;

using Settings_Enums;

[Serializable]
public class Settings_SliderInfo
{
    public Slider slider;
    public SETTINGS_KEY key;

    public Action<SETTINGS_KEY, float> onSliderChanged;

    public void StartListeningForEvents()
    {
        slider.onValueChanged.AddListener(SliderChanged);
    }

    public void StopListeningForEvents()
    {
        slider.onValueChanged.RemoveAllListeners();
    }

    private void SliderChanged(float val)
    {
        onSliderChanged?.Invoke(key, slider.value);
    }
}
