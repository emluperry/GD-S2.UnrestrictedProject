using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;
using UI_Enums;
using Scene_Enums;
using UnityEngine.InputSystem;
using UnityEngine.Device;

public class UI_Manager : MonoBehaviour
{
    [SerializeField] private ScriptableObj_UIDatabase _UIPairs;
    private Dictionary<UI_SCREENS, GameObject> _UIPrefabs = new Dictionary<UI_SCREENS, GameObject>();
    private GameObject _loadScreenInstance;

    private Stack<UI_Screen> _uiStack = new Stack<UI_Screen>();

    public UI_PauseHandler pauseHandler { private set; get; }
    private Dictionary<string, InputAction> _uiInputActions;

    public Action<SCENES, int> onChangeScene;
    public Action onLoadUI;
    public Action<UI_Screen> onLoadSettings;

    private void Awake()
    {
        foreach(UI_ScreenInfo pair in _UIPairs.UIPairs)
        {
            _UIPrefabs.Add(pair.screen, pair.prefab);
        }

        pauseHandler = GetComponent<UI_PauseHandler>();

        SetupPreexistingUI();
    }

    public void SetUIInputActions(Dictionary<string, InputAction> inputs)
    {
        _uiInputActions = inputs;
        pauseHandler.SetInputActions(inputs);
        inputs["Cancel"].performed += Handle_CancelPressed;
    }

    public void SetupPreexistingUI()
    {
        UI_Screen[] currentScreens = FindObjectsOfType<UI_Screen>();
        foreach (UI_Screen screen in currentScreens)
        {
            _uiStack.Push(screen);
            StartListeningForEvents(screen);
            screen.SetupInput(_uiInputActions);
        }
    }

    public void SetupPauseUI(bool allowPausing)
    {
        if(allowPausing)
        {
            pauseHandler.onLoadPause += HandlePauseEvent;
        }
        else
        {
            pauseHandler.onLoadPause -= HandlePauseEvent;
        }
    }

    private void OnDestroy()
    {
        ClearUIStack();
    }

    private void LoadUI(UI_SCREENS screen)
    {
        if(_uiStack.Count > 0)
        {
            _uiStack.Peek().DeactivateInput();
            _uiStack.Peek().gameObject.SetActive(false);
        }

        if (screen == UI_SCREENS.BACK)
        {
            if (_uiStack.Peek().screenType == UI_SCREENS.PAUSE)
            {
                pauseHandler.TogglePause();
            }
            else
            {
                UnloadUIScreen(_uiStack.Pop());

                if (_uiStack.Count > 0)
                {
                    _uiStack.Peek().ActivateInput();
                    _uiStack.Peek().gameObject.SetActive(true);
                }
            }
        }
        else
        {
            UI_Screen uiScreen = Instantiate(_UIPrefabs[screen], transform).GetComponent<UI_Screen>();
            uiScreen.SetupInput(_uiInputActions);

            _uiStack.Push(uiScreen);

            StartListeningForEvents(uiScreen);

            if(screen == UI_SCREENS.PAUSE)
            {
                onLoadSettings?.Invoke(uiScreen);
            }
        }

        onLoadUI?.Invoke();
    }

    private void UnloadUIScreen(UI_Screen screen)
    {
        if(screen != null)
        {
            StopListeningForEvents(screen);
            screen.DeactivateInput();

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

    private void StartListeningForEvents(UI_Screen screen)
    {
        screen.onChangeUIScreen += LoadUI;
        screen.onChangeScene += CallSceneChange;
    }

    private void StopListeningForEvents(UI_Screen screen)
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

    private void Handle_CancelPressed(InputAction.CallbackContext ctx)
    {
        LoadUI(UI_SCREENS.BACK);
    }
}
