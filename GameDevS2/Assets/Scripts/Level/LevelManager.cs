using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Scene_Enums;

public class LevelManager : MonoBehaviour
{
    public PlayerInitialiser player;
    public BattleManager battleManager;

    [SerializeField] private Exit_Pair[] _levelExits;

    public Action<SCENES, int> onLoadLevel;

    private void Awake()
    {
        foreach(Exit_Pair pair in _levelExits)
        {
            pair.StartListeningForEvents();
            pair.onExitTouched += LoadLevel;
        }
    }

    private void OnDestroy()
    {
        foreach (Exit_Pair pair in _levelExits)
        {
            pair.StopListeningForEvents();
            pair.onExitTouched -= LoadLevel;
        }
    }

    private void LoadLevel(int level)
    {
        onLoadLevel?.Invoke(SCENES.LEVEL, level);
    }

    public void SetPlayerSpawn(int lastLevel)
    {
        if (_levelExits.Length <= 0)
            return;

        LevelTeleport startPoint = null;
        //find pair with corresponding level
        foreach(Exit_Pair exit in _levelExits)
        {
            if(exit.levelNumber == lastLevel)
            {
                startPoint = exit.exit;
                break;
            }
        }

        if(startPoint == null) //failsafe: if no corresponding exit found, pick first exit from list
        {
            startPoint = _levelExits[0].exit;
        }

        //move player object there
        player.transform.position = startPoint.playerStartTransform.position;
        player.transform.rotation = startPoint.playerStartTransform.rotation;
    }
}
