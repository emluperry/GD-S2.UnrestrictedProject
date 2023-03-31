using System;

[Serializable]
public class Exit_Pair
{
    public LevelTeleport exit;
    public int levelNumber;

    public Action<int> onExitTouched;

    public void StartListeningForEvents()
    {
        exit.onExitTouched += CallExitTouched;
    }

    public void StopListeningForEvents()
    {
        exit.onExitTouched -= CallExitTouched;
    }

    private void CallExitTouched()
    {
        onExitTouched?.Invoke(levelNumber);
    }
}
