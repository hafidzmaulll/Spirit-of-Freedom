using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class BossEnemy : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public GameObject bullet;
    public Transform bulletPos;

    private GameObject player;
    private float attackTimer;
    private float modeSwitchTimer;

    public float walkSpeed = 3f;
    public float chargeSpeed = 6f;
    public float shootCooldown = 2f;
    public float modeSwitchInterval = 3f; // Interval to switch between attack modes
    public enum AttackMode { Charge, Shoot }
    private AttackMode currentAttackMode = AttackMode.Charge;

    public enum WalkableDirection { Right, Left }
    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;

    public WalkableDirection WalkDirection
    {
        get { return _walkDirection; }
        set
        {
            if (_walkDirection != value)
            {
                // Direction flipped
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if (value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                }
                else if (value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value;
        }
    }

    private bool _hasTarget = false;
    public bool HasTarget
    {
        get { return _hasTarget; }
        private set
        {
            _hasTarget = value;
            if (animator != null)
            {
                animator.SetBool(AnimationStrings.hasTarget, value);
            }
            else
            {
                Debug.LogError("Animator component is not assigned.");
            }
        }
    }

    public bool CanMove
    {
        get
        {
            return animator != null && animator.GetBool(AnimationStrings.canMove);
        }
    }

    public float AttackCooldown
    {
        get
        {
            return animator.GetFloat(AnimationStrings.attackCooldown);
        }
        private set
        {
            animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
        }
    }

    private void Awake()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody2D>();
        touchingDirections = GetComponent<TouchingDirections>();
        damageable = GetComponent<Damageable>();

        // Null checks for essential components
        if (animator == null) Debug.LogError("Animator component is missing.");
        if (rb == null) Debug.LogError("Rigidbody2D component is missing.");
        if (touchingDirections == null) Debug.LogError("TouchingDirections component is missing.");
        if (damageable == null) Debug.LogError("Damageable component is missing.");
        if (attackZone == null) Debug.LogError("AttackZone is not assigned.");
        if (cliffDetectionZone == null) Debug.LogError("CliffDetectionZone is not assigned.");
    }

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
    }

    void Update()
    {
        if (attackZone != null)
        {
            HasTarget = attackZone.detectedColliders.Count > 0;
        }
        else
        {
            Debug.LogError("AttackZone is not assigned.");
        }

        modeSwitchTimer += Time.deltaTime;

        if (modeSwitchTimer >= modeSwitchInterval)
        {
            modeSwitchTimer = 0;
            SwitchAttackMode();
        }

        if (currentAttackMode == AttackMode.Shoot)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= shootCooldown)
            {
                attackTimer = 0;
                Shoot();
            }
        }

        // Set canMove based on the current attack mode
        animator.SetBool(AnimationStrings.canMove, currentAttackMode == AttackMode.Charge && HasTarget);
    }

    private void FixedUpdate()
    {
        if (touchingDirections.IsGrounded && touchingDirections.IsOnWall || cliffDetectionZone.detectedColliders.Count == 0)
        {
            FlipDirection();
        }

        if (!damageable.LockVelocity)
        {
            if (CanMove && touchingDirections.IsGrounded)
            {
                // Adjust speed based on whether a target is detected and attack mode
                float currentSpeed = HasTarget ? (currentAttackMode == AttackMode.Charge ? chargeSpeed : 0) : walkSpeed;

                rb.velocity = new Vector2(currentSpeed * walkDirectionVector.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
        }
    }

    private void SwitchAttackMode()
    {
        if (currentAttackMode == AttackMode.Charge)
        {
            currentAttackMode = AttackMode.Shoot;
        }
        else
        {
            currentAttackMode = AttackMode.Charge;
        }
    }

    private void FlipDirection()
    {
        if (WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        }
        else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        }
        else
        {
            Debug.LogError("Invalid WalkDirection value.");
        }
    }

    private void FlipDirectionToPlayer()
    {
        if (player == null) return;

        Vector2 directionToPlayer = player.transform.position - transform.position;

        if (directionToPlayer.x > 0 && transform.localScale.x < 0)
        {
            // Face right
            transform.localScale = new Vector2(Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
        else if (directionToPlayer.x < 0 && transform.localScale.x > 0)
        {
            // Face left
            transform.localScale = new Vector2(-Mathf.Abs(transform.localScale.x), transform.localScale.y);
        }
    }

    void Shoot()
    {
        Instantiate(bullet, bulletPos.position, Quaternion.identity);
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }

    public void OnCliffDetected()
    {
        if (touchingDirections != null && touchingDirections.IsGrounded)
        {
            FlipDirection();
        }
    }
}
