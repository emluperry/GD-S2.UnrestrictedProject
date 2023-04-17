using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityJump : MonoBehaviour, IPausable, IGroundable
{
    protected Coroutine _jumpingCoroutine;
    protected bool _isGrounded = true;

    //components
    protected Rigidbody _rb;

    [Header("Jumping")]
    [SerializeField][Min(0f)] protected float _minJumpForce = 3500f; //min height: 3.5? max height: 5?
    [SerializeField][Min(0f)] protected float _extraJumpForce = 50f;
    [SerializeField][Min(0f)] protected float _maxButtonHoldDuration = 1f;

    protected bool _isPaused = false;

    protected void Awake()
    {
        _rb = GetComponent<Rigidbody>();
    }

    public void UpdateGrounded(bool isGrounded)
    {
        _isGrounded = isGrounded;
    }

    protected virtual void FixedUpdate()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
        {
            if (!_isGrounded)
            {
                _isGrounded = true;
            }
        }
        else
        {
            if (_isGrounded)
            {
                _isGrounded = false;
            }
        }
    }

    protected void StartJump()
    {
        _isGrounded = false;

        _rb.AddForce(Vector3.up * _minJumpForce);
    }


    protected virtual IEnumerator c_JumpingCoroutine()
    {
        yield return new WaitForFixedUpdate();

        StartJump();

        _jumpingCoroutine = null;
    }

    public void PauseGame(bool isPaused)
    {
        _isPaused = isPaused;
        _rb.useGravity = !isPaused;
    }
}
