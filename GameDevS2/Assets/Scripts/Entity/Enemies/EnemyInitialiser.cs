using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitialiser : MonoBehaviour
{
    //Component references
    private HealthBar _enemyHealthBar;
    private UI_WorldCanvasRotation _canvasRotationComponent;
    private EnemyMovement _enemyMovement;
    private EnemyAnimation _enemyAnimation;
    private EnemySound _enemySound;
    private EnemyAttack _enemyAttack;
    public EntityHealth health { private set; get; }

    private State_Manager _stateMachine;

    private CameraMovement _cameraReference;

    [HideInInspector] public bool isDeactivated { private set; get; } = false;

    private void Awake()
    {
        _stateMachine = GetComponent<State_Manager>();
        _canvasRotationComponent = GetComponentInChildren<UI_WorldCanvasRotation>();
        _enemyHealthBar = GetComponentInChildren<HealthBar>();
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
        _canvasRotationComponent.SetupCameraReference(_enemyHealthBar.transform.parent);

        _enemyMovement.onUpdateRotation += _canvasRotationComponent.UpdateCanvasRotation;
        if (camera.TryGetComponent(out _cameraReference))
        {
            _cameraReference.onCameraRotationUpdated += _canvasRotationComponent.UpdateCanvasRotation;
        }

        _stateMachine.StartBehaviour(player, _enemyMovement, _enemyAttack, _enemyAnimation, _enemySound);

        isDeactivated = false;
    }

    public void DisableEnemy()
    {
        health.onDamageTaken -= _enemyHealthBar.TakeDamage;
        health.onValueIncreased -= UpdateValue;

        _enemyMovement.onUpdateRotation -= _canvasRotationComponent.UpdateCanvasRotation;
        if (_cameraReference)
        {
            _cameraReference.onCameraRotationUpdated -= _canvasRotationComponent.UpdateCanvasRotation;
        }

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
