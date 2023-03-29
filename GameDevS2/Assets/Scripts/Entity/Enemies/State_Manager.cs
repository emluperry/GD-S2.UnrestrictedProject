using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class State_Manager : MonoBehaviour, IPausable
{
    [SerializeField] private Scriptable_State _currentState;

    //other components - public in order for states to access it
    [HideInInspector] public EnemyMovement enemyMovement;
    [HideInInspector] public EnemyAttack enemyAttack;
    //pathfinding details?
    [HideInInspector] public EntityHealth playerHealthTarget;

    public float stateTimeElapsed;

    private bool _isPaused = false;
    private bool _isActive = false;

    private Coroutine _behaviourCoroutine;

    private void Awake()
    {
        //get components
        enemyMovement = GetComponent<EnemyMovement>();
        enemyAttack = GetComponent<EnemyAttack>();
    }

    //setup pathfinding ai?

    public void StartBehaviour(EntityHealth player)
    {
        //playerHealthTarget = player;
        //_isActive = true;

        //_behaviourCoroutine = StartCoroutine(c_BehaviourUpdate());
    }

    public void StopBehaviour()
    {
        _isActive = false;

        if(_behaviourCoroutine != null)
        {
            StopCoroutine(_behaviourCoroutine);
            _behaviourCoroutine = null;
        }
    }

    protected IEnumerator c_BehaviourUpdate()
    {
        while (_isActive)
        {
            yield return new WaitUntil(() => !_isPaused); //freeze execution until not paused
            yield return new WaitForFixedUpdate(); //then wait for fixed update

            _currentState.UpdateState(this);
        }
    }

    public void TransitionToState(Scriptable_State newState)
    {
        if(newState != _currentState)
        {
            OnExitState();

            _currentState = newState;
        }
    }

    public bool IncrementElapsedTime(float duration)
    {
        stateTimeElapsed += Time.fixedDeltaTime;

        return stateTimeElapsed >= duration;
    }

    private void OnExitState()
    {
        _currentState.ExitState(this);
        stateTimeElapsed = 0;
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }
}
