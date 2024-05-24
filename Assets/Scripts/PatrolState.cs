using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolState : MonoBehaviour
{
    public Rigidbody2D rb;
    public Transform ledgeDetector;
    public LayerMask groundLayer;

    private bool facingRight = true;
    public float speed;
    public float raycastDistance;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit2D hit = Physics2D.Raycast(ledgeDetector.position, Vector2.down, raycastDistance, groundLayer);

        if(hit.collider == null)
        {
            Rotate();
        }
    }
    
    void FixedUpdate()
    {
        if(facingRight)
        {
            rb.velocity = new Vector2(speed, rb.velocity.y);
        } else
        {
            rb.velocity = new Vector2(-speed, rb.velocity.y);
        }
            
    }

    void Rotate()
    {
        transform.Rotate(0, 180, 0);
    }
}
