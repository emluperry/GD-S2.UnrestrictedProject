using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UI_Enums;
using Scene_Enums;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private ScriptableObj_UIDatabase _UIPairs;
    private Dictionary<UI_SCREENS, GameObject> _UIPrefabs = new Dictionary<UI_SCREENS, GameObject>();
    private GameObject _loadScreenInstance;

    private Stack<UI_Screen_Buttons> _uiStack = new Stack<UI_Screen_Buttons>();

    public Action<SCENES, int> onChangeScene;
    public Action onLoadUI;
    public Action<UI_Screen_Buttons> onLoadSettings;

    private void Awake()
    {
        foreach(UI_ScreenInfo pair in _UIPairs.UIPairs)
        {
            _UIPrefabs.Add(pair.screen, pair.prefab);
        }

        SetupPreexistingUI();
    }

    public void SetupPreexistingUI()
    {
        UI_Screen_Buttons[] currentScreens = FindObjectsOfType<UI_Screen_Buttons>();
        foreach (UI_Screen_Buttons screen in currentScreens)
        {
            _uiStack.Push(screen);
            StartListeningForEvents(screen);
        }
    }

    public void SetupPauseUI(bool allowPausing)
    {
        if(allowPausing)
            GetComponent<UI_PauseHandler>().onLoadPause += HandlePauseEvent;
        else
            GetComponent<UI_PauseHandler>().onLoadPause -= HandlePauseEvent;
    }

    private void OnDestroy()
    {
        ClearUIStack();
    }

    private void LoadUI(UI_SCREENS screen)
    {
        if(_uiStack.Count > 0)
            _uiStack.Peek().gameObject.SetActive(false);

        if (screen == UI_SCREENS.BACK)
        {
            UnloadUIScreen(_uiStack.Pop());

            if (_uiStack.Count > 0)
                _uiStack.Peek().gameObject.SetActive(true);
        }
        else
        {
            UI_Screen_Buttons uiScreen = Instantiate(_UIPrefabs[screen], transform).GetComponent<UI_Screen_Buttons>();

            _uiStack.Push(uiScreen);

            StartListeningForEvents(uiScreen);

            if(screen == UI_SCREENS.PAUSE)
            {
                onLoadSettings?.Invoke(uiScreen);
            }
        }

        onLoadUI?.Invoke();
    }

    private void UnloadUIScreen(UI_Screen_Buttons screen)
    {
        if(screen != null)
        {
            StopListeningForEvents(screen);

            Destroy(screen.gameObject);
        }
    }

    public void ShowLoadScreen(bool showScreen)
    {
        if (showScreen)
            _loadScreenInstance = Instantiate(_UIPrefabs[UI_SCREENS.LOADING], transform);
        else if (_loadScreenInstance != null)
        {
            Destroy(_loadScreenInstance);
            _loadScreenInstance = null;
        }
    }

    private void ClearUIStack()
    {
        while (_uiStack.Count > 0)
        {
            UnloadUIScreen(_uiStack.Pop());
        }
        _uiStack.Clear();
    }

    private void StartListeningForEvents(UI_Screen_Buttons screen)
    {
        screen.onChangeUIScreen += LoadUI;
        screen.onChangeScene += CallSceneChange;
    }

    private void StopListeningForEvents(UI_Screen_Buttons screen)
    {
        screen.onChangeUIScreen -= LoadUI;
        screen.onChangeScene -= CallSceneChange;
    }

    private void CallSceneChange(SCENES scene, int levelNum)
    {
        ClearUIStack();
        onChangeScene?.Invoke(scene, levelNum);
    }

    private void HandlePauseEvent(bool showPause)
    {
        if(showPause)
        {
            LoadUI(UI_SCREENS.PAUSE);
        }
        else
        {
            ClearUIStack();
        }
    }
}
