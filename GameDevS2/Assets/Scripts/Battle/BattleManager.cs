using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("External References")]
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

        _playerObject.SetEnemyList(enemiesHealth);

        _currentLivingEnemies = enemiesHealth.Length;
        foreach(EntityHealth health in enemiesHealth)
        {
            health.onDead += CheckBattleState;
        }
    }

    private void CheckBattleState()
    {
        _currentLivingEnemies--;

        if (_currentLivingEnemies <= 0)
        {
            _activeSpawner.Deactivate();
            _currentLivingEnemies = 0;
            _activeSpawner = null;
        }
    }
}
