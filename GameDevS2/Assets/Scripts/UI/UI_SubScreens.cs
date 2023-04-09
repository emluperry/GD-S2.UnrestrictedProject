using UnityEngine;

public class UI_SubScreens : UI_Screen
{
    protected UI_SubScreenInfo _currentScreen;

    [SerializeField] protected UI_SubScreenInfo[] _subScreens;

    protected override void Awake()
    {
        base.Awake();

        foreach (UI_SubScreenInfo pair in _subScreens)
        {
            pair.button.SetupElement();
            pair.StartListeningForEvents();
            pair.onButtonClicked += ChangeScreen;
            pair.DeactivateScreen();
        }

        _currentScreen = _subScreens[0];
    }

    protected virtual void Start()
    {
        _currentScreen.ActivateScreen();
    }

    protected override void OnDestroy()
    {
        base.OnDestroy();

        foreach (UI_SubScreenInfo pair in _subScreens)
        {
            pair.StopListeningForEvents();
            pair.onButtonClicked -= ChangeScreen;
        }
    }

    protected void ChangeScreen(UI_SubScreenInfo newScreen)
    {
        _currentScreen.DeactivateScreen();

        newScreen.ActivateScreen();
        _currentScreen = newScreen;
    }
}
