using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private HUD_Manager _hudManager;

    [SerializeField] private Transform _playerObject;
    private PlayerInitialiser _player;
    [SerializeField] private Transform _spawnerObjects;

    private EnemySpawner _activeSpawner;
    private int _currentLivingEnemies = 0;

    public Action onGameOver;

    private void Awake()
    {
        _player = _playerObject.GetComponent<PlayerInitialiser>();

        foreach(Transform child in _spawnerObjects)
        {
            bool hasSpawner = child.TryGetComponent(out EnemySpawner spawner);

            if(hasSpawner)
            {
                spawner.onEnemiesSpawned += StartBattle;
            }
        }
    }

    private void Start()
    {
        _player.health.onDamageTaken += UpdateHUDDamge;
        _player.health.onValueIncreased += UpdateHUDValue;
        _player.health.onDead += GameOver;
    }

    private void OnDestroy()
    {
        _player.health.onDamageTaken -= UpdateHUDDamge;
        _player.health.onValueIncreased -= UpdateHUDValue;
        _player.health.onDead -= GameOver;
    }

    private void StartBattle(EnemySpawner spawner)
    {
        if (_playerObject == null)
        {
            Debug.LogError("NULL PLAYER REFERENCE");
            return;
        }

        _activeSpawner = spawner;

        EnemyInitialiser[] enemyList = _activeSpawner.GetEnemyArray();

        //setup player & HUD
        _player.targeting.SetEnemyList(enemyList);
        _player.cards.InitialiseBattle();

        //setup HUD
        _hudManager.StartBattle(_player.cards.GetDeckList(), _player.cards.GetDeckSize(), _player.health);

        _player.cards.StartBattle(); //draws hand for ui to update

        //setup enemies
        _currentLivingEnemies = enemyList.Length;

        foreach (EnemyInitialiser enemy in enemyList)
        {
            enemy.health.onDead += CheckBattleState;
            enemy.SetupEnemy(_playerObject, _player.movement.GetCameraTransform());
        }
    }

    private void EndBattle()
    {
        EnemyInitialiser[] enemyList = _activeSpawner.GetEnemyArray();

        foreach (EnemyInitialiser enemy in enemyList)
        {
            DeactivateEnemy(enemy);
        }

        _activeSpawner.Deactivate();
        _currentLivingEnemies = 0;
        _activeSpawner = null;

        _player.cards.EndBattle();
        _hudManager.EndBattle();
    }

    private void CheckBattleState(EntityHealth deadEnemy)
    {
        _currentLivingEnemies--;
        DeactivateEnemy(deadEnemy.GetComponent<EnemyInitialiser>());

        if (_currentLivingEnemies <= 0)
        {
            EndBattle();
        }
    }

    private void DeactivateEnemy(EnemyInitialiser enemy)
    {
        if (enemy.isDeactivated)
            return;

        enemy.health.onDead -= CheckBattleState;
        enemy.DisableEnemy();
    }

    private void UpdateHUDDamge(int dmg)
    {
        _hudManager.OnPlayerDamaged(dmg);
    }

    private void UpdateHUDValue(bool whichValue, int amount)
    {
        _hudManager.UpdateHUDValue(whichValue, amount);
    }

    private void GameOver(EntityHealth playerHealthComp)
    {
        onGameOver?.Invoke();
        EndBattle();
    }
}
