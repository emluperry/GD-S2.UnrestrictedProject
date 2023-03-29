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
    private EntityHealth[] _enemyObjects;

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

        PlayerCards playerCardsComponent = _playerObject.GetComponent<PlayerCards>();

        //setup HUD
        if (_hudManager != null)
        {
            _hudManager.StartBattle(playerCardsComponent.GetDeckList());
        }

        //setup player
        _playerObject.SetEnemyList(enemiesHealth);
        playerCardsComponent.StartBattle();

        //setup enemies
        _currentLivingEnemies = enemiesHealth.Length;
        _enemyObjects = enemiesHealth;
        foreach(EntityHealth health in enemiesHealth)
        {
            health.onDead += CheckBattleState;
            health.GetComponent<State_Manager>().StartBehaviour(_playerObject.GetComponent<EntityHealth>());
        }
    }

    private void EndBattle()
    {
        foreach (EntityHealth health in _enemyObjects)
        {
            if (health.isDeactivated)
                continue;

            health.onDead += CheckBattleState;
            health.GetComponent<State_Manager>().StopBehaviour();
            health.isDeactivated = true;
        }

        _activeSpawner.Deactivate();
        _currentLivingEnemies = 0;
        _activeSpawner = null;

        _playerObject.GetComponent<PlayerCards>().EndBattle();
        _hudManager.EndBattle();
    }

    private void CheckBattleState(EntityHealth deadEnemy)
    {
        _currentLivingEnemies--;
        deadEnemy.onDead -= CheckBattleState;
        deadEnemy.GetComponent<State_Manager>().StopBehaviour();
        deadEnemy.isDeactivated = true;

        if (_currentLivingEnemies <= 0)
        {
            EndBattle();
        }
    }
}
