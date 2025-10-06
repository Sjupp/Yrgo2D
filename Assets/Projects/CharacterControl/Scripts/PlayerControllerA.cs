using UnityEngine;
using UnityEngine.InputSystem;

//This script is a clean powerful solution to a top-down movement player
public class PlayerControllerA : MonoBehaviour
{
    [Header("Movement")]
    public float maxSpeed = 5; //Our max speed
    public float acceleration = 20; //How fast we accelerate
    public float deceleration = 4; //brake power


    [Header("Jump")]
    public float jumpPower = 8; //How strong our jump is
    public int maxJumps = 2; //We added a double jump to the game
    public float groundCheckDistance = 0.05f; //how far outside our character we should raycast
    public float extraGravity = 3; //Makes the jump feel better.
    public float defaultGravity = 2;

    //Private variables
    Rigidbody2D rb2D; //Ref to our rigidbody
    float xVelocity; //Our current x-velocity
    float feetOffset; //Length of the raycast
    int currentJumps = 0; //remaining jumps, also works as ground check

    // references to the input actions of the new Input System
    private InputAction moveAction;
    private InputAction jumpAction;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>(); //assign our ref.

        //Using new input system
        moveAction = InputSystem.actions.FindAction("Player/Move");
        jumpAction = InputSystem.actions.FindAction("Player/Jump");

        //Requires enabling because actions are disabled by default
        moveAction.Enable();
        jumpAction.Enable();

        //setting so raycasts don't hit the object they start in.
        Physics2D.queriesStartInColliders = false;

        //Calculate player size based on our colliders, length of raycast
        feetOffset = GetComponent<Collider2D>().bounds.extents.y;// + 0.02f;
    }

    void Update()
    {
        GravityAdjust(); //adjusts gravity
        HorizontalMovement(); //Handles Horizontal movement
        GroundCheck(); //check if we are on ground
        Jump(); //handles jump
    }

    private void GravityAdjust()
    {
        //If we are falling down increase gravity x3
        //This creates a much better feeling, less floaty
        if (rb2D.linearVelocity.y < 0)
            rb2D.gravityScale = extraGravity;
        else
            rb2D.gravityScale = defaultGravity;
    }

    private void HorizontalMovement()
    {
        //Read hoziontal input value
        float x = moveAction.ReadValue<Vector2>().x;

        //If x is anything other than 0, we're either pressing left or right
        if (x != 0)
        {
            //Lerp towards max speed in x direction
            xVelocity = Mathf.Lerp(xVelocity, x * maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            //Lerp towards 0 velocity if we're not sending any input
            xVelocity = Mathf.Lerp(xVelocity, 0f, deceleration * Time.deltaTime);
        }

        //Now we can move with the rigidbody and we get proper collisions
        rb2D.linearVelocity = new Vector2(xVelocity, rb2D.linearVelocity.y);
    }

    private void GroundCheck()
    {
        //Calculate our ray start position
        var rayPos = transform.position;
        rayPos.y -= feetOffset;

        //Fire a raycast
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.down, groundCheckDistance);

        //Debug draw our ray so we can see it.
        Debug.DrawRay(rayPos, Vector2.down * groundCheckDistance);

        // If it hits something...
        if (hit.collider != null)
            currentJumps = 0;
    }

    private void Jump()
    {
        //if we press the button and have jumps remaining
        if (jumpAction.WasPressedThisFrame() && currentJumps < maxJumps)
        {
            currentJumps++;
            //Apply our jump power in the y direction
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpPower);
        }

        if (jumpAction.WasReleasedThisFrame() && rb2D.linearVelocity.y > 0)
        {
            //Cut the jump short by reducing the upward velocity.
            //Same result as button down but on one line.
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0f);
        }
    }
}