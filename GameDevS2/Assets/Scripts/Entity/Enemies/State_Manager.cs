using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class State_Manager : MonoBehaviour, IPausable
{
    [SerializeField] private Scriptable_State _currentState;

    //other components - public in order for states to access it
    public EnemyMovement enemyMovement { get; private set; }
    public EnemyAttack enemyAttack { get; private set; }
    public EnemyAnimation enemyAnimation { get; private set; }
    //pathfinding details?
    public Transform playerTransform { get; private set; }

    public float stateTimeElapsed;

    private bool _isPaused = false;
    private bool _isActive = false;

    private Coroutine _behaviourCoroutine;

    //setup pathfinding ai?

    public void StartBehaviour(Transform player, EnemyMovement movement, EnemyAttack attack, EnemyAnimation anim)
    {
        enemyMovement = movement;
        enemyAttack = attack;
        enemyAnimation = anim;

        if (_currentState == null)
            return;

        playerTransform = player;
        _isActive = true;

        _behaviourCoroutine = StartCoroutine(c_BehaviourUpdate());
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
