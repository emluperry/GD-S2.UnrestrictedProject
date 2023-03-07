using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Collider m_Trigger;

    [SerializeField] private GameObject[] _enemiesToSpawn;
    private EntityHealth[] _enemiesHealth;
    private int _currentLivingEnemies;

    private void Awake()
    {
        m_Trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        _currentLivingEnemies = _enemiesToSpawn.Length;

        _enemiesHealth = new EntityHealth[_currentLivingEnemies];
        for (int i = 0; i < _currentLivingEnemies; i++)
        {
            GameObject obj = Instantiate(_enemiesToSpawn[i], transform);
            _enemiesHealth[i] = obj.GetComponent<EntityHealth>();
            _enemiesHealth[i].onDead += CheckBattleState;
        }

        //start battle
        foreach (Transform child in transform)
        {
            child.gameObject.SetActive(true);
        }

        m_Trigger.enabled = false;
    }

    private void CheckBattleState()
    {
        _currentLivingEnemies--;

        if(_currentLivingEnemies <= 0)
        {
            foreach (Transform child in transform)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
