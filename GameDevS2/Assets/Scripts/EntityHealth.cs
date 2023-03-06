using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 10;
    private int _health = 0;

    public Action onDead;

    private void Awake()
    {
        _health = _maxHealth;
    }

    public void TakeDamage(int dmg)
    {
        _health -= dmg;

        if(_health <= 0)
        {
            //die
            onDead?.Invoke();
        }
    }
}
