using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    Rigidbody2D rb;
    Vector2 moveInput;
    public float walkSpeed = 5f;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {

    }

    void Update()
    {

    }

    private void FixedUpdate()
    {
        rb.velocity = new Vector2(moveInput.x * walkSpeed, rb.velocity.y);
    }

    public void OnMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();

        IsMoving = moveInput != Vector2.zero;   
    }
}