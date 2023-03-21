using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private HUD_Manager _hudManager;

    [SerializeField] private PlayerTargeting _playerObject;
    [SerializeField] private Transform _spawnerObjects;

    private EnemySpawner _activeSpawner;
    private int _currentLivingEnemies = 0;

    private void Awake()
    {
        foreach(Transform child in _spawnerObjects)
        {
            bool hasSpawner = child.TryGetComponent(out EnemySpawner spawner);

            if(hasSpawner)
            {
                spawner.onEnemiesSpawned += StartBattle;
            }
        }
    }

    private void StartBattle(EnemySpawner spawner, EntityHealth[] enemiesHealth)
    {
        if (_playerObject == null)
        {
            Debug.LogError("NULL PLAYER REFERENCE");
            return;
        }

        _activeSpawner = spawner;

        //setup player
        _playerObject.SetEnemyList(enemiesHealth);
        PlayerCards playerCardsComponent = _playerObject.GetComponent<PlayerCards>();
        playerCardsComponent.StartBattle();

        //setup HUD
        if (_hudManager != null)
        {
            _hudManager.StartBattle(playerCardsComponent.GetDeckList());
        }

        //setup enemies
        _currentLivingEnemies = enemiesHealth.Length;
        foreach(EntityHealth health in enemiesHealth)
        {
            health.onDead += CheckBattleState;
        }
    }

    private void EndBattle()
    {
        _activeSpawner.Deactivate();
        _currentLivingEnemies = 0;
        _activeSpawner = null;

        _playerObject.GetComponent<PlayerCards>().EndBattle();
        _hudManager.EndBattle();
    }

    private void CheckBattleState()
    {
        _currentLivingEnemies--;

        if (_currentLivingEnemies <= 0)
        {
            EndBattle();
        }
    }
}
