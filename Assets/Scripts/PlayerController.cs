using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;


[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour
{
    [SerializeField] private float maxSpeed;
    [SerializeField] private float movePower;
    [SerializeField] private float jumpPower;

    private Rigidbody2D rb;
    private Animator animator;
    private SpriteRenderer render;
    private Vector2 inputDir;
    private bool isGround;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
        render = rb.GetComponent<SpriteRenderer>();
    }

    private void Update()
    {
        Move();
    }

    private void Move()
    {
        if(inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
        /*rb.AddForce(Vector2.right * inputDir.x * movePower * Time.deltaTime, ForceMode2D.Impulse); »óµ¿*/
        else if (inputDir.x > 0 && rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
    }

    private void OnMove(InputValue Value)
    {
        inputDir = Value.Get<Vector2>();
        animator.SetFloat("MoveSpeed", Mathf.Abs(inputDir.x));
        if(inputDir.x > 0)
        {
            render.flipX = false;
        }
        else if(inputDir.x < 0)
        {
            render.flipX = true;
        }
    }

    private void Jump()
    {
        if (!animator.GetBool("Jump")) 
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("Jump", true);
        }
    }

    private void OnJump(InputValue Value)
    {
        Jump();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetBool("Jump", false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("Jump",true);
    }
}
