using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Base stuff")]
    public Rigidbody2D rb2D;
    public float BASE_MOVE_SPEED;
    public Sprite[] sprite;
    private SpriteRenderer actualSprite;
    private Vector2 vectorInput;
    private int facing;

    [Header("Jump Attributes")]
    public Transform groundCheck;
    public float checkRadius;
    public float jumpForce;
    public LayerMask whatIsGround;
    private bool isGrounded;

    [Header("Wall related abilities")]
    public Transform rightGrab;
    public Transform leftGrab;
    public float grabCheckRadius;
    public float wallJumpForce;
    public float climbSpeed;
    private bool canGrab;
    private bool canGrabLeft;
    private bool isGrabing;
    private bool isSliding;
    private bool canWallJump;

    [Header("Other")]
    public float dashMultiplier;
    public float startDashTime;
    private float dashTime;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
        actualSprite = gameObject.GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        if(this)
        {
            if(Input.GetKeyDown("space"))
            {
                PlayerJump();
                rb2D.gravityScale = 1f;
            }
            //Grab stuff
            canGrab = Physics2D.OverlapCircle(rightGrab.position, grabCheckRadius, whatIsGround);
            canGrabLeft = Physics2D.OverlapCircle(leftGrab.position, grabCheckRadius, whatIsGround);    //Both check if there's a wall left or right

            if(canGrab || canGrabLeft)
            {
                canWallJump = true;
                if(canGrabLeft)
                {
                    if(Input.GetKey("l"))
                    {
                        PlayerGrab();
                    }
                    else if(vectorInput.x < 0)
                    {
                        WallSlide();
                    }
                }
                else if(canGrab)
                {
                    if(Input.GetKey("l"))
                    {
                        PlayerGrab();
                    }
                    else if(vectorInput.x > 0)
                    {
                        WallSlide();
                    }
                }
            }            
            if(!isGrabing)
            {
                MovePlayer();
                rb2D.gravityScale = 1f;
                if(Input.GetKeyDown("k"))
                {
                    Dash();
                }
            }
            isGrabing = false;
            isSliding = false;
            FacingSprite();
        }
    }

    void FixedUpdate()
    {
       if(this)
       {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            vectorInput.x = Input.GetAxisRaw("Horizontal");
            vectorInput.y = Input.GetAxisRaw("Vertical");
       }
    }

    private void MovePlayer()
    {
        rb2D.velocity = new Vector2(vectorInput.x * BASE_MOVE_SPEED, rb2D.velocity.y);
    }

    private void MovePlayerVertical()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, vectorInput.y * climbSpeed);
    }

    private void FacingSprite()
    {
        if(vectorInput.x > 0)
        {
            SwapSprite(2);
            facing = 1;
        }
        if(vectorInput.x < 0)
        {
            SwapSprite(1);
            facing = 0;
        }
        else if(vectorInput.x == 0)
        {
            SwapSprite(0);
        }
    }

    private void SwapSprite(int spriteNb)
    {
        actualSprite.sprite = sprite[spriteNb];
    }

    private void PlayerJump()
    {
        print("is sliding = " + isSliding);
        if(isSliding)
        {
            print(isSliding + "slide");
            WallSlideJump();
        }        
        else if(isGrounded)
        {
            rb2D.velocity = Vector2.up * jumpForce;
        }
    }

    private void PlayerGrab()
    {
        isGrabing = true;
        rb2D.velocity = new Vector2(0f, 0f);
        rb2D.gravityScale = 0f;
        if(vectorInput.y != 0)
        {
            MovePlayerVertical();
        }
        if(Input.GetKey("space") && canWallJump)       //seems to work properly instead of GetKeyDown
        {
            WallJump();
            canWallJump = false;
        }
    }

    private void WallJump()
    {
        //rb2D.gravityScale = 1f;
        rb2D.velocity = new Vector2(vectorInput.x * BASE_MOVE_SPEED, 1f * jumpForce);
    }

    private void WallSlide()
    {
        if(!isGrabing)
        {
            rb2D.velocity = Vector2.up * -0.3f;
            isSliding = true;
        }
    }

    private void WallSlideJump()
    {
        print("wallslide jump : " + rb2D.velocity);
        rb2D.velocity = new Vector2(-vectorInput.x * BASE_MOVE_SPEED, 1f);
    }

    private void Dash()
    {
        Debug.Log("dash");
        rb2D.velocity = vectorInput * dashMultiplier;
        //rb2D.AddForce(rb2D.position + vectorInput * dashMultiplier);
        //rb2D.MovePosition(rb2D.position + vectorInput * dashMultiplier);
    }
}