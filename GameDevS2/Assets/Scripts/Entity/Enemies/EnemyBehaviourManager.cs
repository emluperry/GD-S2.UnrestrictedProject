using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourManager : MonoBehaviour
{
    [SerializeField][Range(0f, 20f)] private float _minBehaviourSwapDelay = 5f;
    [SerializeField] private float _maxBehaviourSwapDelay = 10f;

    [SerializeField] private EnemyBehaviour[] _behaviourList;
    private int _currentBehaviour = 0;

    private Coroutine _behaviourCoroutine;

    //try to set this up as a state machine, with 'enemy behaviour' as the base state class

    public void StartBehaviour()
    {
        _behaviourCoroutine = StartCoroutine(c_BehaviourUpdate());
    }

    public void StopBehaviour()
    {
        StopCoroutine(_behaviourCoroutine);
    }

    protected IEnumerator c_BehaviourUpdate()
    {
        yield return new WaitForFixedUpdate();
    }
}
