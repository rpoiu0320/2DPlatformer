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

    [SerializeField] LayerMask groundLayer;

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

    private void FixedUpdate()
    {
        GroundCheck();
    }

    private void Move()
    {
        if(inputDir.x < 0 && rb.velocity.x > -maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
        /*rb.AddForce(Vector2.right * inputDir.x * movePower * Time.deltaTime, ForceMode2D.Impulse); 상동*/

        else if (inputDir.x > 0 && rb.velocity.x < maxSpeed)
            rb.AddForce(Vector2.right * inputDir.x * movePower, ForceMode2D.Force);
    }

    private void OnMove(InputValue Value)
    {
        inputDir = Value.Get<Vector2>();
        animator.SetFloat("MoveSpeed", Mathf.Abs(inputDir.x));  // 절댓값 받아와서 계산
        if(inputDir.x > 0)
        {
            render.flipX = false;                               // 렌더를 반전시킬지 아닐지  
        }
        else if(inputDir.x < 0)
        {
            render.flipX = true;
        }
    }

    private void Jump()
    {
        if (animator.GetBool("IsGround")) 
        {
            rb.AddForce(Vector2.up * jumpPower, ForceMode2D.Impulse);
            animator.SetBool("IsGround", false);
        }
    }

    private void OnJump(InputValue Value)
    {
        if (!isGround)
            return;
        Jump();
    }

    private void GroundCheck()
    {
        RaycastHit2D hit = Physics2D.Raycast(transform.position, Vector2.down, 1.05f, groundLayer);
        if(hit.collider != null)
        {
            Debug.DrawRay(transform.position, new Vector3(hit.point.x, hit.point.y, 0) - transform.position, Color.yellow);
            isGround = true;
            animator.SetBool("IsGround", true );
        }
        else
        {
            isGround = false;
            animator.SetBool("IsGround", false);
            Debug.DrawRay(transform.position, Vector3.down * 1f, Color.yellow);
        }
    }

    /*private void OnCollisionEnter2D(Collision2D collision)
    {
        animator.SetBool("IsGround", false);
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        animator.SetBool("IsGround", true);
    }*/
}
