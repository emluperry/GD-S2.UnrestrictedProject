using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitialiser : MonoBehaviour
{
    //Component references
    private HealthBar _enemyHealthBar;
    private EnemyMovement _enemyMovement;
    private EnemyAnimation _enemyAnimation;
    private EnemySound _enemySound;
    private EnemyAttack _enemyAttack;
    public EntityHealth health { private set; get; }

    private State_Manager _stateMachine;

    [HideInInspector] public bool isDeactivated { private set; get; } = false;

    private void Awake()
    {
        _stateMachine = GetComponent<State_Manager>();

        _enemyHealthBar = GetComponentInChildren<HealthBar>();
        health = GetComponent<EntityHealth>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyAnimation = GetComponent<EnemyAnimation>();
        _enemySound = GetComponent<EnemySound>();
        _enemyAttack = GetComponent<EnemyAttack>();
    }

    public void SetupEnemy(Transform player, Transform camera)
    {
        _enemyAnimation.SetupValues(_enemyMovement.GetMaxSpeed());

        health.onDamageTaken += _enemyHealthBar.TakeDamage;
        health.onValueIncreased += UpdateValue;

        _enemyHealthBar.SetupBar(health.GetMaxHealth());
        _enemyMovement.SetupCanvasReference(_enemyHealthBar.transform.parent, camera);

        _stateMachine.StartBehaviour(player, _enemyMovement, _enemyAttack, _enemyAnimation, _enemySound);

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
