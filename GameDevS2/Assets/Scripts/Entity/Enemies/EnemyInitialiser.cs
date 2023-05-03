using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyInitialiser : MonoBehaviour
{
    //Component references
    protected HealthBar _enemyHealthBar;
    protected UI_WorldCanvasRotation _canvasRotationComponent;
    protected EnemyMovement _enemyMovement;
    protected EnemyAnimation _enemyAnimation;
    protected EnemySound _enemySound;
    protected EnemyAttack _enemyAttack;
    public EntityHealth health { protected set; get; }

    protected State_Manager _stateMachine;

    protected CameraMovement _cameraReference;

    [HideInInspector] public bool isDeactivated { protected set; get; } = false;

    protected virtual void Awake()
    {
        _stateMachine = GetComponent<State_Manager>();
        _enemyHealthBar = GetComponentInChildren<HealthBar>();
        _canvasRotationComponent = _enemyHealthBar.GetComponentInParent<UI_WorldCanvasRotation>();
        health = GetComponent<EntityHealth>();
        _enemyMovement = GetComponent<EnemyMovement>();
        _enemyAnimation = GetComponent<EnemyAnimation>();
        _enemySound = GetComponent<EnemySound>();
        _enemyAttack = GetComponent<EnemyAttack>();
    }

    public virtual void SetupEnemy(Transform player, Transform camera)
    {
        _enemyAnimation.SetupValues(_enemyMovement.GetMaxSpeed());

        health.onDamageTaken += _enemyHealthBar.TakeDamage;
        health.onValueIncreased += UpdateValue;

        _enemyHealthBar.SetupBar(health.GetMaxHealth());
        _canvasRotationComponent.SetupCameraReference(camera);

        _enemyMovement.onUpdateRotation += _canvasRotationComponent.UpdateCanvasRotation;
        if (camera.TryGetComponent(out _cameraReference))
        {
            _cameraReference.onCameraRotationUpdated += _canvasRotationComponent.UpdateCanvasRotation;
        }

        _stateMachine.StartBehaviour(player, _enemyMovement, _enemyAttack, _enemyAnimation, _enemySound);

        isDeactivated = false;
    }

    public virtual void DisableEnemy()
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

    protected virtual void UpdateValue(bool whichValue, int amount)
    {
        if (whichValue)
            _enemyHealthBar.HealHealth(amount);
        else
            _enemyHealthBar.IncreaseShield(amount);
    }
}
