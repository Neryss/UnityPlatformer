using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [Header("Base stuff")]
    public Rigidbody2D rb2D;
    public float BASE_MOVE_SPEED;
    public Animator animator;
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
    private bool canSlideJump;

    [Header("Dash")]
    public float setSpeedMultiplier;
    private float speedMultiplier;
    public float tempSpeedMultiplier;
    private float keyPressTime;
    private bool isDashing;

    // Start is called before the first frame update
    void Start()
    {
        rb2D = gameObject.GetComponent<Rigidbody2D>();
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
            isSliding = false;
            if(Input.GetKeyDown("k"))
            {
                //store the time at which the key was pressed and look if it is less that itself + 2s
                //if so it executes the dash
                //Also store the base speedMultiplier that I use to boost the velocity and so, to dash
                keyPressTime = Time.time;
                if(Time.time < keyPressTime + 2) //Store this value somewhere and put it in the second if statement
                {
                    print("time is less that keypressed and 2s, here's the key press time :" + keyPressTime);
                    print("key time plus 2s : " + (keyPressTime + 2));
                    Dash();
                    print(speedMultiplier);
                }
                else  //Need to put another if statement with the ending keytime
                {
                    EndDash();
                }
                isDashing = false;
                print("speed mult is :" + speedMultiplier);
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
                        canSlideJump = true;
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
                        canSlideJump = true;
                        WallSlide();
                    }
                }
            }
            if(!isGrabing && !isDashing && !isSliding)
            {
                rb2D.gravityScale = 1f;
                MovePlayer();
            }
            //print("velocity :" + rb2D.velocity);
            isGrabing = false;
        }
    }

    void FixedUpdate()
    {
       if(this)
       {
            isGrounded = Physics2D.OverlapCircle(groundCheck.position, checkRadius, whatIsGround);
            vectorInput.x = Input.GetAxisRaw("Horizontal");
            vectorInput.y = Input.GetAxisRaw("Vertical");
            animator.SetFloat("Horizontal", vectorInput.x);
       }
    }

    private void MovePlayer()
    {
        rb2D.velocity = new Vector2(vectorInput.x * BASE_MOVE_SPEED + speedMultiplier, rb2D.velocity.y);
    }

    private void MovePlayerVertical()
    {
        rb2D.velocity = new Vector2(rb2D.velocity.x, vectorInput.y * climbSpeed);
    }

    private void PlayerJump()
    {
        if(isGrounded)
        {
            rb2D.velocity = Vector2.up * jumpForce;
            Debug.Log("ground jump");
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
            if(Input.GetKey("space") && canSlideJump)
            {
                WallSlideJump();
                canSlideJump = false;
                Debug.Log("wallslide jump");
            }
        }
    }

    private void WallSlideJump()
    {
        rb2D.velocity = new Vector2(-20, 1 * jumpForce);
        print("wallslide jump : " + rb2D.velocity);
        //rb2D.AddForce(new Vector2(-100, 10));
    }

    private void Dash()
    {
        //isDashing = true;
        //rb2D.velocity = new Vector2(100, 0);
        //print("dash!");
        speedMultiplier = setSpeedMultiplier;
    }

    private void EndDash()
    {
        speedMultiplier = 0;
        print("end dash");
    }
}