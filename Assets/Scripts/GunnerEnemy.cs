using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections), typeof(Damageable))]
public class GunnerEnemy : MonoBehaviour
{
    AudioSource audioSource;
    Animator animator;
    Rigidbody2D rb;
    TouchingDirections touchingDirections;
    Damageable damageable;

    public DetectionZone attackZone;
    public GameObject bullet;
    public Transform bulletPos;
    private GameObject player;
    private float timer;

    public float detectionRange = 10f; // Range within which the gunner detects the player

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
    }

    
     void Start()
    {
        audioSource = GetComponent<AudioSource>();
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

        float distance = Vector2.Distance(transform.position, player.transform.position);
        Debug.Log(distance);

        if (distance < detectionRange)
        {
            FlipDirectionToPlayer();

            timer += Time.deltaTime;

            if (timer > 2)
            {
                timer = 0;
                Shoot();
            }
        }
    }

    private void FixedUpdate()
    {
        // No movement logic required
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
        audioSource.Play();
    }

    public void OnHit(float damage, Vector2 knockback)
    {
        rb.velocity = new Vector2(knockback.x, rb.velocity.y + knockback.y);
    }
}
