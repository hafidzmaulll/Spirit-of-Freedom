using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D), typeof(TouchingDirections))]
public class ChargerEnemy : MonoBehaviour
{
    Rigidbody2D rb;
    Animator animator;
    TouchingDirections touchingDirections;

    private Vector2 walkDirectionVector = Vector2.right;
    private WalkableDirection _walkDirection;
    public DetectionZone attackZone;
    public DetectionZone cliffDetectionZone;
    public float walkSpeed = 3f;
    public float walkSpeedOnAttack = 6f;
    public enum WalkableDirection { Right, Left }

    public WalkableDirection WalkDirection
    {
        get 
        {  
            return _walkDirection;
        } set 
        { 
            if(_walkDirection != value)
            {
                // Membalik Arah
                gameObject.transform.localScale = new Vector2(gameObject.transform.localScale.x * -1, gameObject.transform.localScale.y);

                if(value == WalkableDirection.Right)
                {
                    walkDirectionVector = Vector2.right;
                } else if(value == WalkableDirection.Left)
                {
                    walkDirectionVector = Vector2.left;
                }
            }
            _walkDirection = value; 
        }
    }

    public bool _hasTarget = false;
    public bool HasTarget { get
    {
        return _hasTarget;
    } private set
    {
        _hasTarget = value;
        animator.SetBool(AnimationStrings.hasTarget, value);
    } }

    // public float AttackCooldown { get
    // {
    //     return animator.GetFloat(AnimationStrings.attackCooldown);
    // } private set
    // {
    //     animator.SetFloat(AnimationStrings.attackCooldown, Mathf.Max(value, 0));
    // } }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        touchingDirections = GetComponent<TouchingDirections>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        HasTarget = attackZone.detectedColliders.Count > 0;

        // if(AttackCooldown > 0)
        // {
        //     AttackCooldown -= Time.deltaTime;
        // }
            
    }

    private void FixedUpdate()
    {
        if(touchingDirections.IsGrounded && touchingDirections.IsOnWall)
        {
            FlipDirection();
        }

        if(HasTarget)
        {
            rb.velocity = new Vector2(walkSpeedOnAttack * walkDirectionVector.x, rb.velocity.y);
        } else
        {
            rb.velocity = new Vector2(walkSpeed * walkDirectionVector.x, rb.velocity.y);
        }
    }

    private void FlipDirection()
    {
        if(WalkDirection == WalkableDirection.Right)
        {
            WalkDirection = WalkableDirection.Left;
        } else if (WalkDirection == WalkableDirection.Left)
        {
            WalkDirection = WalkableDirection.Right;
        } else
        {
            Debug.LogError("ERROR");
        }
    }
}
