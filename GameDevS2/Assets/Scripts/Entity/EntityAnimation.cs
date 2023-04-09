using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityAnimation : MonoBehaviour, IPausable, IGroundable
{
    protected Coroutine _moveCoroutine;

    //components
    protected Animator _animator;
    protected Rigidbody _rb;

    //constraints
    [Header("Motion")]
    [SerializeField] private AnimationCurve _locomotionCurve;
    protected float _maxSpeed = 0f;
    protected bool _isGrounded = true;

    protected bool _isPaused = false;

    protected virtual void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();
    }

    protected virtual void OnDestroy()
    {

    }

    public virtual void SetupValues(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
    }

    protected virtual void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            _isGrounded = true;
            _animator.SetBool("Fall_b", false);
        }
        else
        {
            _isGrounded = false;
            if (_rb.velocity.y < 0)
                _animator.SetBool("Fall_b", true);
        }
    }

    protected virtual void StartMovement()
    {
        if (_moveCoroutine == null && _maxSpeed > 0)
        {
            _moveCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    protected virtual IEnumerator c_MovementCoroutine()
    {
        yield return new WaitForSeconds(0.05f);

        Vector2 velocity = new Vector2(_rb.velocity.x, _rb.velocity.z);

        while (velocity.sqrMagnitude > 0)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            float moveSpeed = velocity.magnitude;
            moveSpeed = Mathf.Clamp01(moveSpeed / _maxSpeed);

            _animator.SetFloat("Speed_f", _locomotionCurve.Evaluate(moveSpeed));

            velocity = new Vector2(_rb.velocity.x, _rb.velocity.z);
        }

        _animator.SetFloat("Speed_f", 0);
        _moveCoroutine = null;
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
    }

    public void OnDead(EntityHealth health)
    {
        _animator.SetBool("Dead_b", true);
    }

    public void UpdateGrounded(bool isGrounded)
    {
        _isGrounded = isGrounded;

        _animator.SetBool("Fall_b", !_isGrounded);
    }
}
