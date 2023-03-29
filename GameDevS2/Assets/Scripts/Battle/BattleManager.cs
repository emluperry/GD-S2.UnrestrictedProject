using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class BattleManager : MonoBehaviour
{
    [Header("External References")]
    [SerializeField] private HUD_Manager _hudManager;

    [SerializeField] private Transform _playerObject;
    private EntityHealth _playerHealth;
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

        _playerHealth = _playerObject.GetComponent<EntityHealth>();

        _playerHealth.onDamageTaken += UpdateHUDDamge;
        _playerHealth.onValueIncreased += UpdateHUDValue;
        _playerHealth.onDead += GameOver;
    }

    private void StartBattle(EnemySpawner spawner)
    {
        if (_playerObject == null)
        {
            Debug.LogError("NULL PLAYER REFERENCE");
            return;
        }

        _activeSpawner = spawner;

        GameObject[] enemyList = _activeSpawner.GetEnemyArray();

        PlayerCards playerCardsComponent = _playerObject.GetComponent<PlayerCards>();

        //setup HUD
        if (_hudManager != null)
        {
            _hudManager.StartBattle(playerCardsComponent.GetDeckList(), playerCardsComponent.GetDeckSize(), _playerHealth);
        }

        //setup player
        _playerObject.GetComponent<PlayerTargeting>().SetEnemyList(enemyList);
        playerCardsComponent.StartBattle();

        //setup enemies
        _currentLivingEnemies = enemyList.Length;

        foreach (GameObject enemy in enemyList)
        {
            enemy.GetComponent<EntityHealth>().onDead += CheckBattleState;

            enemy.GetComponent<State_Manager>().StartBehaviour(_playerObject);
        }
    }

    private void EndBattle()
    {
        GameObject[] enemyList = _activeSpawner.GetEnemyArray();

        foreach (GameObject enemy in enemyList)
        {
            DeactivateEnemy(enemy.GetComponent<EntityHealth>());
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
        DeactivateEnemy(deadEnemy);

        if (_currentLivingEnemies <= 0)
        {
            EndBattle();
        }
    }

    private void DeactivateEnemy(EntityHealth enemy)
    {
        if (enemy.isDeactivated)
            return;

        enemy.onDead -= CheckBattleState;
        enemy.GetComponent<State_Manager>().StopBehaviour();
        enemy.isDeactivated = true;
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
        EndBattle();
    }
}
