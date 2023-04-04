using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using static UnityEngine.GraphicsBuffer;

public class EntityAnimation : MonoBehaviour, IInput, IPausable
{
    //input actions
    private InputAction _moveInput;
    private InputAction _jumpInput;
    private InputAction _attackInput;

    private Coroutine _moveCoroutine;

    //components
    private Animator _animator;
    private Rigidbody _rb;
    private PlayerAttack _playerAttack;

    //constraints
    private float _maxSpeed = 0f;
    private bool _isGrounded = true;

    private bool _isPaused = false;

    private void Awake()
    {
        _animator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody>();

        _playerAttack = GetComponent<PlayerAttack>();
        _playerAttack.onCardUsed += Handle_AttackAnimation;
    }

    private void OnDestroy()
    {
        DisableInput();
    }

    public void SetupValues(float maxSpeed)
    {
        _maxSpeed = maxSpeed;
    }

    #region INPUT SETUP
    public void SetupInput(Dictionary<string, InputAction> inputs)
    {
        _moveInput = inputs["Move"];
        _jumpInput = inputs["Jump"];

        EnableInput();
    }

    public void EnableInput()
    {
        if (_moveInput != null)
        {
            _moveInput.performed += Handle_MovePerformed;
            _jumpInput.performed += Handle_JumpPerformed;
        }
    }

    public void DisableInput()
    {
        if(_moveInput != null)
        {
            _moveInput.performed -= Handle_MovePerformed;
            _jumpInput.performed -= Handle_JumpPerformed;
        }
    }
    #endregion

    #region INPUT

    private void Handle_MovePerformed(InputAction.CallbackContext ctx)
    {
        if(_moveCoroutine == null)
        {
            _moveCoroutine = StartCoroutine(c_MovementCoroutine());
        }
    }

    private void Handle_JumpPerformed(InputAction.CallbackContext ctx)
    {
        if(_isGrounded)
        {
            _animator.SetTrigger("Jump_t");
            _animator.SetBool("Fall_b", false);
            _isGrounded = false;
        }
    }

    private void Handle_AttackAnimation(GDS2_Cards.CARD_TYPE cardType)
    {
        switch (cardType)
        {
            case GDS2_Cards.CARD_TYPE.ATTACK:
                _animator.SetTrigger("Attack_t");
                break;
            case GDS2_Cards.CARD_TYPE.HEALTH:
                _animator.SetTrigger("Magic_t");
                break;
            case GDS2_Cards.CARD_TYPE.DEFENSE:
                _animator.SetTrigger("Shield_t");
                break;
        }
        
    }

    #endregion

    protected void FixedUpdate()
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

    private IEnumerator c_MovementCoroutine()
    {
        if(_maxSpeed <= 0) //do not run if max speed has not been setup as cannot divide by 0
        {
            _moveCoroutine = null;
            yield break;
        }

        while (new Vector2(_rb.velocity.x, _rb.velocity.z).sqrMagnitude > 0)
        {
            yield return new WaitUntil(() => !_isPaused);
            yield return new WaitForFixedUpdate();

            float moveSpeed = _rb.velocity.magnitude;
            moveSpeed = Mathf.Clamp01(moveSpeed / _maxSpeed);

            _animator.SetFloat("Speed_f", moveSpeed);
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
}
