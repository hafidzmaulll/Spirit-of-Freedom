using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class BossStage1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public GameObject bullet;
    public Transform bulletPos;
    private float timer;
    public float walkSpeed = 3f;
    public float walkSpeedOnAttack = 6f; // Speed when charging towards the player
    public float detectionRange = 10f; // Range within which the boss detects the player
    public enum WalkableDirection { Right, Left }

    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;
    private GameObject player;

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

    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        StartCoroutine(AttackCycle());
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

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log(distance);

        if (AttackCooldown > 0)
        {
            AttackCooldown -= Time.deltaTime;
        }

        if (distance < detectionRange)
        {
            FlipDirectionToPlayer();
        }
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
                // Adjust speed based on whether a target is detected
                float currentSpeed = HasTarget ? walkSpeedOnAttack : walkSpeed;
                rb.velocity = new Vector2(currentSpeed * walkDirectionVector.x, rb.velocity.y);
            }
            else
            {
                rb.velocity = new Vector2(0, rb.velocity.y);
            }
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

    IEnumerator AttackCycle()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            // Charge Attack
            animator.SetBool("IsCharging", true);
            yield return new WaitForSeconds(1.5f);
            animator.SetBool("IsCharging", false);

            yield return new WaitForSeconds(1.5f); // Adjust this delay if needed

            // Shooting Attack
            animator.SetBool("IsShooting", true);
            Shoot();
            yield return new WaitForSeconds(1.5f);
            animator.SetBool("IsShooting", false);
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
