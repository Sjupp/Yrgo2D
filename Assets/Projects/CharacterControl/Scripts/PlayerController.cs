using UnityEngine;
using UnityEngine.InputSystem;

//This script is a clean powerful solution to a top-down movement player
public class PlayerController : MonoBehaviour
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

    //Private variables
    Rigidbody2D rb2D; //Ref to our rigidbody
    float xVelocity; //Our current x-velocity
    float feetOffset; //Length of the raycast
    int currentJumps = 0; //remaining jumps, also works as ground check
    private Animator _animator = null;
    [SerializeField] private GameObject _playerSpriteObject = null;

    private bool _useTimer = false;
    private float _timer = 0f;
    private bool _displayTime = false;

    private InputAction m_MoveAction;
    private InputAction m_JumpAction;

    private Vector2 _move = Vector2.zero;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>(); //assign our ref.
        _animator = _playerSpriteObject.GetComponent<Animator>();

        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");

        m_MoveAction.Enable();
        m_JumpAction.Enable();

        //setting so raycasts don't hit the object they start in.
        Physics2D.queriesStartInColliders = false;

        //Calculate player size based on our colliders, length of raycast
        feetOffset = GetComponent<Collider2D>().bounds.extents.y;// + 0.02f;
    }

    void Update()
    {

        _move.x = m_MoveAction.ReadValue<Vector2>().x;

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
            rb2D.gravityScale = 1;
    }

    private void HorizontalMovement()
    {
        if (_move.x != 0)
        {
            xVelocity = Mathf.Lerp(xVelocity, _move.x * maxSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            xVelocity = Mathf.Lerp(xVelocity, 0f, deceleration * Time.deltaTime);
        }

        rb2D.linearVelocity = new Vector2(xVelocity, rb2D.linearVelocity.y);

        _animator.SetFloat("xVelocity", Mathf.Abs(xVelocity / maxSpeed));
    }

    private void GroundCheck()
    {
        //Calculate our ray start position
        var rayPos = transform.position;
        rayPos.y -= feetOffset;

        //Fire a raycast
        RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.down, groundCheckDistance);
        _animator.SetBool("Airborne", !hit);

        //Debug draw our ray so we can see it.
        Debug.DrawRay(rayPos, Vector2.down * groundCheckDistance);

        // If it hits something...
        if (hit.collider != null)
            currentJumps = 0;


    }

    private void Jump()
    {
        //if we press the button and have jumps remaining
        if (m_JumpAction.WasPressedThisFrame() && currentJumps < maxJumps)
        {
            currentJumps++;
            //Apply our jump power in the y direction
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpPower);
        }

        if (m_JumpAction.WasReleasedThisFrame() && rb2D.linearVelocity.y > 0)
        {
            //Cut the jump short by reducing the upward velocity.
            //Same result as button down but on one line.
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
        }
    }
}