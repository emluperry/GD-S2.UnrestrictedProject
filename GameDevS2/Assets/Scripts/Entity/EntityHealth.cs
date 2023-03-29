using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityHealth : MonoBehaviour
{
    [SerializeField] private int _maxHealth = 10;
    private int _health = 0;
    private bool _isDead = false;

    private int _currentShields = 0;

    public Action<bool, int> onValueIncreased; //true for health, false for shields
    public Action<int> onDamageTaken;

    public bool isDeactivated = false;
    public Action<EntityHealth> onDead;

    private void Awake()
    {
        _health = _maxHealth;
    }

    public int GetMaxHealth()
    {
        return _maxHealth;
    }

    private int DamageShield(int dmg)
    {
        if (_currentShields <= 0)
            return dmg;

        _currentShields -= dmg;

        if(_currentShields < 0)
        {
            int remainder = Mathf.Abs(_currentShields);
            _currentShields = 0;
            return remainder;
        }

        return 0;
    }

    public void TakeDamage(int dmg)
    {
        if (_isDead)
            return;

        int remainder = DamageShield(dmg);

        _health -= remainder;
        onDamageTaken?.Invoke(dmg);

        if(_health <= 0)
        {
            _isDead = true;
            onDead?.Invoke(this);
        }
    }

    public void HealHealth(int amount)
    {
        if (_isDead)
            return;

        _health += amount;
        _health = Mathf.Clamp(_health, 0, _maxHealth);

        onValueIncreased?.Invoke(true, amount);
    }

    public void IncreaseShield(int amount)
    {
        _currentShields += amount;

        onValueIncreased?.Invoke(false, amount);
    }

    public bool GetIsDead()
    {
        return _isDead;
    }
}
