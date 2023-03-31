using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Scene_Enums;

public class Scene_Manager : MonoBehaviour
{
    [SerializeField] private ScriptableObj_SceneDatabase _sceneDatabase;

    [SerializeField] private UI_Manager _uiManager;
    [SerializeField] private Settings_Manager _settingsManager;
    [SerializeField] private Input_Manager _inputManager;
    private LevelManager _levelManager = null;

    private int _previousLevel = -1;
    private int _currentLevel = -1;

    [Header("DEBUG")]
    [SerializeField] private bool _debugSetupLevelListeners = false;

    private void Awake()
    {
        if(_uiManager == null)
        {
            _uiManager = FindObjectOfType<UI_Manager>();
        }
        if (_settingsManager == null)
        {
            _settingsManager = FindObjectOfType<Settings_Manager>();
        }
        if (_inputManager == null)
        {
            _inputManager = FindObjectOfType<Input_Manager>();
        }

        if (_uiManager)
        {
            _uiManager.onChangeScene += LoadScene;

            if(_settingsManager)
            {
                _uiManager.onLoadSettings += _settingsManager.ListenForSettingsUI;
                _uiManager.onLoadUI += _settingsManager.SetSceneValues;
            }
        }
    }

    private void Start()
    {
        if (SceneManager.sceneCount == 1)
        {
            AsyncOperation loadOp = SceneManager.LoadSceneAsync(_sceneDatabase.GetSceneName(SCENES.START), LoadSceneMode.Additive);
            loadOp.completed += SceneLoaded;
        }

        if (_uiManager && _inputManager)
        {
            _uiManager.SetUIInputActions(_inputManager.GetUIInputActions());

            if(_debugSetupLevelListeners) //DEBUG ONLY FUNCTION: necessary to test levels without going through extra scenes
            {
                SetupLevelListeners();
            }
        }
    }

    private void OnDestroy()
    {
        if (_uiManager)
        {
            _uiManager.onChangeScene -= LoadScene;

            if(_settingsManager)
            {
                _uiManager.onLoadSettings -= _settingsManager.ListenForSettingsUI;
                _uiManager.onLoadUI -= _settingsManager.SetSceneValues;
            }
        }
    }

    private void LoadScene(SCENES sceneEnum, int levelNum)
    {
        string SceneName;

        if (sceneEnum == SCENES.QUIT)
        {
            QuitApp();
            return;
        }
        else
        {
            if(_currentLevel > -1)
                RemoveLevelListeners();

            _previousLevel = _currentLevel;
            _currentLevel = levelNum;

            if(sceneEnum == SCENES.LEVEL)
            {
                SceneName = _sceneDatabase.GetLevelName(levelNum);
            }
            else
            {
                SceneName = _sceneDatabase.GetSceneName(sceneEnum);
            }
        }

        _uiManager.ShowLoadScreen(true);

        SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());

        AsyncOperation loadOp = SceneManager.LoadSceneAsync(SceneName, LoadSceneMode.Additive);

        loadOp.completed += SceneLoaded;

        _uiManager.SetupPauseUI(_sceneDatabase.IsScenePausable(sceneEnum, levelNum));
    }

    private void SceneLoaded(AsyncOperation op)
    {
        SceneManager.SetActiveScene(SceneManager.GetSceneAt(SceneManager.sceneCount - 1));

        _settingsManager.SetSceneValues();
        _uiManager.ShowLoadScreen(false);

        _uiManager.SetupPreexistingUI();

        if(_currentLevel > -1)
        {
            SetupLevelListeners();
        }

        op.completed -= SceneLoaded;
    }

    private void SetupLevelListeners()
    {
        _levelManager = FindObjectOfType<LevelManager>();
        _levelManager.onLoadLevel += LoadScene;
        _levelManager.battleManager.onGameOver += _uiManager.HandleGameOver;
        _levelManager.player.InitialisePauseEvents(ref _uiManager.pauseHandler.onLoadPause);
        _levelManager.SetPlayerSpawn(_previousLevel);

        _inputManager.SetupLevelInput();
    }

    private void RemoveLevelListeners()
    {
        if(_levelManager)
        {
            _levelManager.onLoadLevel -= LoadScene;
            _levelManager.battleManager.onGameOver -= _uiManager.HandleGameOver;
            _levelManager.player.StopListeningForPause(ref _uiManager.pauseHandler.onLoadPause);
        }
    }

    private void QuitApp()
    {
        Application.Quit();
    }
}
