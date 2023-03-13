using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Collider _trigger;
    [SerializeField] private GameObject _walls;

    [SerializeField] private GameObject[] _enemiesToSpawn;

    public Action<EnemySpawner, EntityHealth[]> onEnemiesSpawned;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        EntityHealth[] enemiesHealth = new EntityHealth[_enemiesToSpawn.Length];
        for (int i = 0; i < _enemiesToSpawn.Length; i++)
        {
            EntityHealth health = Instantiate(_enemiesToSpawn[i], transform).GetComponent<EntityHealth>();
            enemiesHealth[i] = health;
        }

        onEnemiesSpawned?.Invoke(this, enemiesHealth);

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
