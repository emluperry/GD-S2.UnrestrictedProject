using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Enemy_Tutorial : EnemyInitialiser
{
    [SerializeField] private UI_Tutorial_Input_Pair[] _TutorialSections;
    private Coroutine _tutorialCoroutine;
    private bool _tutorialSectionComplete = false;
    [SerializeField] private float _successDuration = 3;

    protected override void Awake()
    {
        _enemyHealthBar = GetComponentInChildren<HealthBar>();
        _canvasRotationComponent = _enemyHealthBar.GetComponentInParent<UI_WorldCanvasRotation>();
        health = GetComponent<EntityHealth>();
        _enemySound = GetComponent<EnemySound>();
    }

    public override void SetupEnemy(Transform player, Transform camera)
    {
        health.onDamageTaken += _enemyHealthBar.TakeDamage;
        health.onValueIncreased += UpdateValue;

        _enemyHealthBar.SetupBar(health.GetMaxHealth());
        _canvasRotationComponent.SetupCameraReference(_enemyHealthBar.transform.parent);

        if (camera.TryGetComponent(out _cameraReference))
        {
            _cameraReference.onCameraRotationUpdated += _canvasRotationComponent.UpdateCanvasRotation;
        }

        Dictionary<string, InputAction> playerInputs = player.GetComponent<PlayerInitialiser>().savedInputs;

        isDeactivated = false;

        //start tutorial here
        if(_TutorialSections.Length > 0)
        {
            foreach(UI_Tutorial_Input_Pair pair in _TutorialSections)
            {
                if (pair.TutorialInputName == "CORRECT")
                    continue;

                pair.SetInputAction(playerInputs[pair.TutorialInputName]);
            }

            _tutorialCoroutine = StartCoroutine(c_UpdateTutorial());
        }
    }

    protected override void UpdateValue(bool whichValue, int amount)
    {
        base.UpdateValue(whichValue, amount);
    }

    public override void DisableEnemy()
    {
        health.onDamageTaken -= _enemyHealthBar.TakeDamage;
        health.onValueIncreased -= UpdateValue;

        if (_cameraReference)
        {
            _cameraReference.onCameraRotationUpdated -= _canvasRotationComponent.UpdateCanvasRotation;
        }

        if(_tutorialCoroutine != null)
        {
            StopCoroutine(_tutorialCoroutine);
            _tutorialCoroutine = null;
        }

        isDeactivated = true;
    }

    protected IEnumerator c_UpdateTutorial()
    {
        yield return null;
        _tutorialSectionComplete = false;
        int currentTutorial = 1;

        while (currentTutorial != _TutorialSections.Length)
        {
            _TutorialSections[currentTutorial].TutorialImage.gameObject.SetActive(true);
            _TutorialSections[currentTutorial].TutorialInputAction.performed += CompleteAction;

            yield return new WaitUntil(() => _tutorialSectionComplete);
            _tutorialSectionComplete = false;

            _TutorialSections[currentTutorial].TutorialImage.gameObject.SetActive(false);
            _TutorialSections[currentTutorial].TutorialInputAction.performed -= CompleteAction;
            _TutorialSections[0].TutorialImage.gameObject.SetActive(true);

            yield return new WaitForSecondsRealtime(_successDuration);

            _TutorialSections[0].TutorialImage.gameObject.SetActive(false);
            currentTutorial++;
        }

        _tutorialCoroutine = null;
    }

    protected void CompleteAction(InputAction.CallbackContext ctx)
    {
        _tutorialSectionComplete = true;
    }
}
