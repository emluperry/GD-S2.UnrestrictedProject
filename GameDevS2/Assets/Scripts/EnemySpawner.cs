using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Collider _trigger;
    [SerializeField] private GameObject _walls;

    [SerializeField] private GameObject[] _enemiesToSpawn;
    private EntityHealth[] _enemiesHealth;
    private int _currentLivingEnemies;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        _currentLivingEnemies = _enemiesToSpawn.Length;

        _enemiesHealth = new EntityHealth[_currentLivingEnemies];
        for (int i = 0; i < _currentLivingEnemies; i++)
        {
            GameObject obj = Instantiate(_enemiesToSpawn[i], transform);
            _enemiesHealth[i] = obj.GetComponent<EntityHealth>();
            _enemiesHealth[i].onDead += CheckBattleState;
        }

        other.GetComponent<EnemyTargeting>().SetEnemyList(_enemiesHealth);

        //start battle
        GetComponent<MeshRenderer>().enabled = false;
        _trigger.enabled = false;

        _walls.SetActive(true);
    }

    private void CheckBattleState()
    {
        _currentLivingEnemies--;

        if(_currentLivingEnemies <= 0)
        {
            gameObject.SetActive(false);
        }
    }
}
