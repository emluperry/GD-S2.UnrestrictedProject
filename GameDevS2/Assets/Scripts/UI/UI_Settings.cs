using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Settings_Enums;
using System;

public class UI_Settings : MonoBehaviour
{
    [SerializeField] private Settings_SliderInfo[] _sliders;
    private Dictionary<SETTINGS_KEY, Slider> _sliderDictionary = new Dictionary<SETTINGS_KEY, Slider>();

    public Action onDestroyed;
    public Action<SETTINGS_KEY> updateValue;

    private void Awake()
    {
        foreach(Settings_SliderInfo pair in _sliders)
        {
            _sliderDictionary.Add(pair.key, pair.slider);
            pair.StartListeningForEvents();
            pair.onSliderChanged += UpdateSliderSettings;
            SetSliderValue(pair.key, PlayerPrefs.GetFloat(pair.key.ToString()));
        }
    }

    private void OnDestroy()
    {
        foreach (Settings_SliderInfo pair in _sliders)
        {
            pair.StopListeningForEvents();
            pair.onSliderChanged -= UpdateSliderSettings;
        }
        _sliderDictionary.Clear();

        onDestroyed?.Invoke();
    }

    private void UpdateSliderSettings(SETTINGS_KEY key, float val)
    {
        PlayerPrefs.SetFloat(key.ToString(), val);
        updateValue?.Invoke(key);
    }

    private void SetSliderValue(SETTINGS_KEY key, float val)
    {
        _sliderDictionary[key].value = val;
    }
}
