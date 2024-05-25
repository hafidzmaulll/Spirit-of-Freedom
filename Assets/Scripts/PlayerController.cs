using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    Vector2 moveInput;
    TouchingDirections touchingDirections;
    Damageable damageable;
    public float jumpImpulse = 10f;
    public float walkSpeed = 5f;
    public float runSpeed = 8f;

    // Buat Dash
    private float dashingPower = 960f;
    private float dashingTime = 0.2f;
    private float dashingCooldown = 2f;
    private bool canDash = true;
    private bool isDashing;
    [SerializeField] private TrailRenderer tr;

    // Merubah Sebaliknya Arah Player Menghadap
    public bool _isFacingRight = true;
    public bool IsFacingRight { get
    {
        return _isFacingRight;
    } private set
    {
        if(_isFacingRight != value)
        {
            // Merubah Sebaliknya Arah Player Menghadap
            transform.localScale *= new Vector2(-1, 1);
        }
        _isFacingRight = value;
    } }

    // Mendeteksi Pergerakan Player
    public bool CanMove { get
        {
            return animator.GetBool(AnimationStrings.canMove);
        }
    }

    public bool IsAlive
    {
        get
        {
            return animator.GetBool(AnimationStrings.isAlive);
        }
    }

    // Untuk Mengatur Move Speed Player
    public float CurrentMoveSpeed { get
        {
            if(CanMove)
            {
                if(IsMoving && !touchingDirections.IsOnWall)
                {
                    if(IsRunning)
                    {
                        return runSpeed;
                    } else
                    {
                        return walkSpeed;
                    }
                } else
                {
                    // Kecepatan Idle adalah 0
                    return 0;
                }
            } else
            {
                // Pergerakan Dikunci
                return 0;
            }
            
        }
    }

    // Untuk Player Gerak
    [SerializeField]
    private bool _isMoving = false;
    public bool IsMoving { get
    {
        return _isMoving;
    } private set
    {
        _isMoving = value;
        animator.SetBool(AnimationStrings.isMoving, value);
    } }

    // Untuk Player Lari
    [SerializeField]
    private bool _isRunning = false;
    public bool IsRunning { get
    {
        return _isRunning;
    } private set
    {
        _isRunning = value;
        animator.SetBool(AnimationStrings.isRunning, value);
    } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();
    }

    void Start()
    {

    }

    void Update()
    {
        // Buat Dash
        if(isDashing)
        {
            return;
        }

        if(Input.GetKeyDown(KeyCode.LeftControl) && canDash)
        {
            StartCoroutine(Dash());
        }
    }

    private void FixedUpdate()
    {
        if(isDashing)
        {
            return;
        }
        
        if(!damageable.LockVelocity)
            rb.velocity = new Vector2(moveInput.x * CurrentMoveSpeed, rb.velocity.y);

        animator.SetFloat(AnimationStrings.yVelocity, rb.velocity.y);
    }

    // Untuk Mengatur Arah Player Menghadap
    private void SetFacingDirection(Vector2 moveInput)
    {
        if(moveInput.x > 0 && !IsFacingRight)
        {
            // Player Menghadap ke Kanan
            IsFacingRight = true;

        } else if(moveInput.x < 0 && IsFacingRight)
        {
            // Player Menghadap ke Kiri
            IsFacingRight = false;
        }
    }
    // Input Action untuk Player Gerak
    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        if(IsAlive)
        {
            IsMoving = moveInput != Vector2.zero;
            SetFacingDirection(moveInput);
        } else
        {
            IsMoving = false;
        }
    }

    // Input Action untuk Player Lari
    public void OnRun(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            IsRunning = true;
        } else if(context.canceled)
        {
            IsRunning = false;
        }
    }

    // Input Action untuk Player Lompat
    public void OnJump(InputAction.CallbackContext context)
    {
        // Untuk Cek Apakah Player Masih Hidup
        if(context.started && touchingDirections.IsGrounded && CanMove)
        {
            animator.SetTrigger(AnimationStrings.jumpTrigger);
            rb.velocity = new Vector2(rb.velocity.x, jumpImpulse);
        }
    }

    // Input Action untuk Player Dapat Menyerang
    public void OnAttack(InputAction.CallbackContext context)
    {
        if(context.started)
        {
            animator.SetTrigger(AnimationStrings.attackTrigger);
        }
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    // Buat Dash
    private IEnumerator Dash()
    {
        canDash = false;
        isDashing = true;
        float originalGravity = rb.gravityScale;
        rb.gravityScale = 0f;
        rb.velocity = new Vector2(transform.localScale.x * dashingPower, 0f);
        tr.emitting = true;
        yield return new WaitForSeconds(dashingTime);
        tr.emitting = false;
        rb.gravityScale = originalGravity;
        isDashing = false;
        yield return new WaitForSeconds(dashingCooldown);
        canDash = true;
    }
}