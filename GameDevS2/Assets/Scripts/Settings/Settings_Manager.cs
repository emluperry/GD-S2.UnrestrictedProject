using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Settings_Enums;
using UnityEngine.Audio;
using System;

public class Settings_Manager : MonoBehaviour
{
    private UI_Settings _settingsScreen;

    [SerializeField] private AudioMixer _mixer;

    private float _MaxMasterDB;
    private float _MaxMusicDB;
    private float _MaxSFXDB;

    private void Awake()
    {
        _mixer.GetFloat(SETTINGS_KEY.MASTER_VOLUME.ToString(), out _MaxMasterDB);
        _mixer.GetFloat(SETTINGS_KEY.BGM_VOLUME.ToString(), out _MaxMusicDB);
        _mixer.GetFloat(SETTINGS_KEY.SFX_VOLUME.ToString(), out _MaxSFXDB);

        SetAllValuesToPreferences();
    }

    private void SetAllValuesToPreferences()
    {
        _mixer.SetFloat(SETTINGS_KEY.MASTER_VOLUME.ToString(), -80 + ((_MaxMasterDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.MASTER_VOLUME.ToString())));
        _mixer.SetFloat(SETTINGS_KEY.BGM_VOLUME.ToString(), -80 + ((_MaxMusicDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.BGM_VOLUME.ToString())));
        _mixer.SetFloat(SETTINGS_KEY.SFX_VOLUME.ToString(), -80 + ((_MaxSFXDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.SFX_VOLUME.ToString())));

        SetSceneValues();
    }

    public void SetSceneValues()
    {
        
    }

    private void UpdateValueToPreference(SETTINGS_KEY key)
    {
        switch(key)
        {
            case SETTINGS_KEY.MASTER_VOLUME:
                _mixer.SetFloat(SETTINGS_KEY.MASTER_VOLUME.ToString(), -80 + ((_MaxMasterDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.MASTER_VOLUME.ToString())));
                break;
            case SETTINGS_KEY.BGM_VOLUME:
                _mixer.SetFloat(SETTINGS_KEY.BGM_VOLUME.ToString(), -80 + ((_MaxMusicDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.BGM_VOLUME.ToString())));
                break;
            case SETTINGS_KEY.SFX_VOLUME:
                _mixer.SetFloat(SETTINGS_KEY.SFX_VOLUME.ToString(), -80 + ((_MaxSFXDB + 80) * PlayerPrefs.GetFloat(SETTINGS_KEY.SFX_VOLUME.ToString())));
                break;
        }
    }

    public void ListenForSettingsUI(UI_Screen_Buttons screen)
    {
        _settingsScreen = screen.GetComponentInChildren<UI_Settings>(true);
        _settingsScreen.updateValue += UpdateValueToPreference;
        _settingsScreen.onDestroyed += StopListeningToSettingsUI;
    }

    private void StopListeningToSettingsUI()
    {
        _settingsScreen.onDestroyed -= StopListeningToSettingsUI;
        _settingsScreen.updateValue -= UpdateValueToPreference;
        _settingsScreen = null;
    }
}
