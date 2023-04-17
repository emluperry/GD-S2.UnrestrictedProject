using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    private Collider _trigger;
    [SerializeField] private ParticleSystem _particles;
    [SerializeField] private GameObject _walls;

    [SerializeField] private GameObject[] _enemiesToSpawn;
    private EnemyInitialiser[] _enemiesSpawned;

    public Action<EnemySpawner> onEnemiesSpawned;

    private void Awake()
    {
        _trigger = GetComponent<Collider>();
        if(!_particles)
            _particles = GetComponentInChildren<ParticleSystem>();

        _enemiesSpawned = new EnemyInitialiser[_enemiesToSpawn.Length];
        for (int i = 0; i < _enemiesToSpawn.Length; i++)
        {
            EnemyInitialiser enemy = Instantiate(_enemiesToSpawn[i], transform).GetComponent<EnemyInitialiser>();
            enemy.gameObject.SetActive(false);
            _enemiesSpawned[i] = enemy;

        }
    }

    public EnemyInitialiser[] GetEnemyArray()
    {
        return _enemiesSpawned;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
            return;

        foreach (EnemyInitialiser enemy in _enemiesSpawned)
        {
            enemy.gameObject.SetActive(true);
        }

        onEnemiesSpawned?.Invoke(this);

        //start battle
        _particles.Clear();
        _particles.Stop();

        _trigger.enabled = false;

        _walls.SetActive(true);
    }

    public void Deactivate()
    {
        gameObject.SetActive(false);
    }
}
