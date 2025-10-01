using PrimeTween;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

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
    [SerializeField] private ShakeSettings _jumpTween;
    [SerializeField] private bool _useCoyoteTime = false;
    [SerializeField] private float _coyoteTimeDuration = 0.2f;

    //Private variables
    Rigidbody2D rb2D; //Ref to our rigidbody
    float xVelocity; //Our current x-velocity
    float feetOffset; //Length of the raycast
    int currentJumps = 0; //remaining jumps, also works as ground check
    
    private bool _isGrounded = false;
    private bool _isRunning = false;
    private InputAction m_MoveAction;
    private InputAction m_JumpAction;
    private Vector2 _move = Vector2.zero;
    private int _xVelocityStringHash = 0;
    private int _airborneStringHash = 0;
    private Animator _animator = null;
    private SpriteRenderer _spriteRenderer = null;
    private float _coyoteTimer = 0f;
    
    [SerializeField] private GameObject _playerSpriteObject = null;
    [SerializeField] private GameObject _spriteBase = null;
    [SerializeField] private ParticleSystem _dashVFX = null;
    [SerializeField] private ParticleSystem _jumpVFX = null;

    [Header("Juice")]
    [SerializeField] private bool _animation = true;
    [SerializeField] private bool _tweens = true;
    [SerializeField] private bool _vfx = true;

    void Awake()
    {
        rb2D = GetComponent<Rigidbody2D>(); //assign our ref.
        _animator = _playerSpriteObject.GetComponent<Animator>();
        _spriteRenderer = _playerSpriteObject.GetComponent<SpriteRenderer>();

        //Using new input system
        m_MoveAction = InputSystem.actions.FindAction("Player/Move");
        m_JumpAction = InputSystem.actions.FindAction("Player/Jump");

        m_MoveAction.Enable();
        m_JumpAction.Enable();

        //setting so raycasts don't hit the object they start in.
        Physics2D.queriesStartInColliders = false;

        //Calculate player size based on our colliders, length of raycast
        //feetOffset = GetComponent<Collider2D>().bounds.extents.y;// + 0.02f;

        _xVelocityStringHash = Animator.StringToHash("xVelocity");
        _airborneStringHash = Animator.StringToHash("Airborne");
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

        float xVelocityAbsolute = Mathf.Abs(xVelocity);

        bool facingRight = xVelocity != 0 && xVelocity > 0f;
        _spriteRenderer.flipX = facingRight;
        _dashVFX.transform.localScale = facingRight ? new Vector3(-1f, 1f, 1f) : Vector3.one;

        if (_animation)
        {
            _animator.SetFloat(_xVelocityStringHash, Mathf.Abs(xVelocity / maxSpeed));
        }

        if (_vfx)
        {
            if (xVelocityAbsolute > 0.05f && !_isRunning && _isGrounded)
            {
                _dashVFX.Play();
            }
            else if (_isRunning && xVelocityAbsolute <= 0.05f || !_isGrounded)
            {
                _dashVFX.Stop();
            }
        }

        _isRunning = xVelocityAbsolute > 0.05f;
    }

    private void GroundCheck()
    {
        //Calculate our ray start position
        var rayPos = transform.position;
        rayPos.y -= feetOffset;

        //Fire a raycast
        //RaycastHit2D hit = Physics2D.Raycast(rayPos, Vector2.down, groundCheckDistance);
        RaycastHit2D hit = Physics2D.BoxCast(transform.position + Vector3.up, Vector2.one * 1f, 0f, Vector2.down, groundCheckDistance);

        // check to see if we just landed
        if (!_isGrounded && hit)
        {
            if (hit.collider.TryGetComponent(out Platform comp))
            {
                Debug.Log("Landed on color: " + comp._platformColor.ToString());
                var vfxMain = _dashVFX.main;
                vfxMain.startColor = comp._platformColor;
            }

            if (_tweens)
            {
                Tween.PunchScale(_spriteBase.transform, new Vector3(0.3f, -0.2f, 0.3f), 0.2f, 5);
            }

            if (_vfx)
            {
                if (_isRunning)
                {
                    _dashVFX.Play();
                }
            }
        }

        if (_isGrounded && !hit)
        {
            // just jumped or left the ground
            _coyoteTimer = Time.time;
        }

        _isGrounded = hit;

        if (_animation)
        {
            _animator.SetBool(_airborneStringHash, !_isGrounded);
        }

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
            if (_isGrounded)
            {
                // do normal jump from ground

                currentJumps++;
                //Apply our jump power in the y direction
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpPower);

                Debug.Log("Normal time jump");

            }
            else if (_coyoteTimer + _coyoteTimeDuration < Time.time)
            {
                // do coyote time jump

                currentJumps++;
                //Apply our jump power in the y direction
                rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, jumpPower);
                Debug.Log("Coyote time jump");
            }


            //if (_tweens)
            //{
            //    Tween.PunchScale(_spriteBase.transform, _jumpTween);
            //}

            //if (_vfx)
            //{
            //    _jumpVFX.Play();
            //}

        }

        if (m_JumpAction.WasReleasedThisFrame() && rb2D.linearVelocity.y > 0)
        {
            //Cut the jump short by reducing the upward velocity.
            //Same result as button down but on one line.
            rb2D.linearVelocity = new Vector2(rb2D.linearVelocity.x, rb2D.linearVelocity.y * 0.5f);
        }
    }
}