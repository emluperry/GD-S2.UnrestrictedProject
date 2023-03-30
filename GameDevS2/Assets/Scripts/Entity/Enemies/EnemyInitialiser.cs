using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitialiser : MonoBehaviour
{
    //Component references
    private HealthBar _enemyHealthBar;
    private EnemyMovement _enemyMovement;
    public EntityHealth health { private set; get; }

    private State_Manager _stateMachine;

    [HideInInspector] public bool isDeactivated { private set; get; } = false;

    private void Awake()
    {
        _enemyHealthBar = GetComponentInChildren<HealthBar>();
        health = GetComponent<EntityHealth>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _stateMachine = GetComponent<State_Manager>();
    }

    public void SetupEnemy(Transform player, Transform camera)
    {
        health.onDamageTaken += _enemyHealthBar.TakeDamage;
        health.onValueIncreased += UpdateValue;

        _enemyHealthBar.SetupBar(health.GetMaxHealth());
        _enemyMovement.SetupCanvasReference(_enemyHealthBar.transform.parent, camera);

        _stateMachine.StartBehaviour(player);

        isDeactivated = false;
    }

    public void DisableEnemy()
    {
        health.onDamageTaken -= _enemyHealthBar.TakeDamage;
        health.onValueIncreased -= UpdateValue;

        _stateMachine.StopBehaviour();

        isDeactivated = true;
    }

    private void UpdateValue(bool whichValue, int amount)
    {
        if (whichValue)
            _enemyHealthBar.HealHealth(amount);
        else
            _enemyHealthBar.IncreaseShield(amount);
    }
}
