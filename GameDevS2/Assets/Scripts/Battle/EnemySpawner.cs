using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Collider _trigger;
    [SerializeField] private GameObject _walls;

    [SerializeField] private GameObject[] _enemiesToSpawn;
    private GameObject[] _enemiesSpawned;

    public Action<EnemySpawner> onEnemiesSpawned;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();

        _enemiesSpawned = new GameObject[_enemiesToSpawn.Length];
        for (int i = 0; i < _enemiesToSpawn.Length; i++)
        {
            GameObject enemy = Instantiate(_enemiesToSpawn[i], transform);
            enemy.SetActive(false);
            _enemiesSpawned[i] = enemy;

        }
    }

    public GameObject[] GetEnemyArray()
    {
        return _enemiesSpawned;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (GameObject enemy in _enemiesSpawned)
        {
            enemy.SetActive(true);
        }

        onEnemiesSpawned?.Invoke(this);

        //start battle
        GetComponent<MeshRenderer>().enabled = false;
        _trigger.enabled = false;

        _walls.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
