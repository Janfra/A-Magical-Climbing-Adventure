using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerJump : PlayerControllerModule
{
    /// <summary>
    /// Called when player jumps
    /// </summary>
    public event Action OnJump;

    /// <summary>
    /// Called when player lands
    /// </summary>
    public event Action OnGrounded;

    /// <summary>
    /// Calls event only once per grounded
    /// </summary>
    public event Action OnGroundedOnce;

    #region Variables & Constants

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody2D rigidbody;

    private bool isOnGround;
    private bool isOnGroundLastFrame;
    private bool canDoubleJump;

    private float yInputDirection;
    private bool isInputDown = false;

    #region Coyote Time

    private float timeSinceGrounded;
    private bool isCoyoteTime => timeSinceGrounded > 0;

    #endregion

    #region Jump Buffer

    private float timeDelayToJump;
    public float TimeSinceJump { get; private set; }
    private bool isInJumpTime => TimeSinceJump > 0.0f && timeDelayToJump < 0;

    private const float JUMP_DELAY = 0.3f;

    #endregion

    [Header("Configuration")]
    /// <summary>
    /// Sets the force affecting how high the player jumps
    /// </summary>
    [SerializeField]
    private float jumpForce = 1.0f;

    /// <summary>
    /// Changes how fast player gets pulled down after it stops jumping
    /// </summary>
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float jumpCutMultiplier = 0.5f;

    /// <summary>
    /// Layers the player can jump on
    /// </summary>
    [SerializeField]
    private LayerMask jumpableLayer;

    /// <summary>
    /// Position to check for the floor collision
    /// </summary>
    [SerializeField]
    private Transform groundCheckPosition;

    /// <summary>
    /// Size of the circle checking for collision
    /// </summary>
    [SerializeField]
    private float groundCheckRadius = 0.05f;

    /// <summary>
    /// Duration where player is allowed to jump after leaving platform
    /// </summary>
    [SerializeField]
    private float coyoteTime = 0.2f;

    /// <summary>
    /// Duration where the player will jump if the floor is hit after pressing jump button
    /// </summary>
    [SerializeField]
    private float jumpBufferTime = 0.2f;

    /// <summary>
    /// Normal gravity scale to set on rigidbody
    /// </summary>
    [SerializeField]
    private float gravityScale = 1.5f;

    /// <summary>
    /// Multiplier to apply to gravity when falling
    /// </summary>
    [SerializeField]
    private float fallGravityMultiplier = 2.0f;

    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float jumpGravityHaltThreshold;

    [SerializeField]
    private float jumpGravityHaltMultiplier = 0.5f;

    [SerializeField]
    private float downwardBonusFallSpeedMultiplier = 3.0f;

    [SerializeField]
    private float maxFallingSpeed = -12.0f;
    [SerializeField]
    private float maxFallingSpeedDownward = -15.0f;

    #endregion

    /// <summary>
    /// Initializes the class
    /// </summary>
    /// <param name="_controller"></param>
    public override void Init(PlayerController _controller, AMagicalClimbingAdventure _inputs)
    {
        if(groundCheckPosition == null)
        {
            Debug.LogError("No ground check assigned to player controller");
        }
        rigidbody = _controller.PlayerRigidbody;

        _inputs.Player.Jump.started += CheckDoubleJump;
        _inputs.Player.Jump.performed += ResetTimeSinceJump;
        _inputs.Player.Jump.canceled += StopJumping;
        _inputs.Player.Move.performed += UpdateDownwardSpeed;
        _inputs.Player.Move.canceled += UpdateDownwardSpeed;
    }

    /// <summary>
    /// Updates the direction of vertical movement
    /// </summary>
    /// <param name="_context"></param>
    private void UpdateDownwardSpeed(InputAction.CallbackContext _context)
    {
        yInputDirection = _context.ReadValue<Vector2>().y;
    }

    /// <summary>
    /// Runs on update to update logic variables
    /// </summary>
    public override void OnUpdate()
    {
        UpdateJumpBufferAndGravity();
        IsPlayerOnGroundUpdate();
        ClampFallingSpeed();
    }

    /// <summary>
    /// Handles jump logic on fixed update
    /// </summary>
    public void HandleJump()
    {
        if (isCoyoteTime && isInJumpTime)
        {
            Jump();
        }
    }

    /// <summary>
    /// Clears events
    /// </summary>
    public override void UndoInputBinding(AMagicalClimbingAdventure _inputs, bool _isTemporary)
    {
        if (!_isTemporary)
        {
            OnGrounded = null;
            OnGroundedOnce = null;
            OnJump = null;
        }

        yInputDirection = 0.0f;
        _inputs.Player.Jump.started -= CheckDoubleJump;
        _inputs.Player.Jump.performed -= ResetTimeSinceJump;
        _inputs.Player.Jump.canceled -= StopJumping;
        _inputs.Player.Move.performed -= UpdateDownwardSpeed;
        _inputs.Player.Move.canceled -= UpdateDownwardSpeed;
    }

    /// <summary>
    /// Sets whether the player can double jump
    /// </summary>
    public void SetDoubleJump(bool _canDoubleJump)
    {
        canDoubleJump = _canDoubleJump;
        timeDelayToJump = JUMP_DELAY;
    }

    /// <summary>
    /// Attempt to double jump
    /// </summary>
    private void CheckDoubleJump(InputAction.CallbackContext _context)
    {
        if (canDoubleJump && JUMP_DELAY > 0 && !isOnGround)
        {
            Jump();
        }
    }

    /// <summary>
    /// Updates the jump buffer variables, restarting it or substracting the timer
    /// </summary>
    private void UpdateJumpBufferAndGravity()
    {
        TimeSinceJump -= Time.deltaTime;
        timeDelayToJump -= Time.deltaTime;

        if (Mathf.Abs(rigidbody.velocity.y) < jumpGravityHaltThreshold)
        {
            rigidbody.gravityScale = gravityScale * jumpGravityHaltMultiplier;
            return;
        }

        if (yInputDirection < 0)
        {
            isInputDown = true;
            rigidbody.gravityScale = gravityScale * downwardBonusFallSpeedMultiplier;
            return;
        }
        else
        {
            isInputDown = false;
        }

        // Fall faster when going down
        if (rigidbody.velocity.y < 0)
        {
            rigidbody.gravityScale = gravityScale * fallGravityMultiplier;
        }
        else
        {
            rigidbody.gravityScale = gravityScale;
        }
    }

    /// <summary>
    /// Resets the time since jump to the buffer time
    /// </summary>
    private void ResetTimeSinceJump(InputAction.CallbackContext _context)
    {
        TimeSinceJump = jumpBufferTime;
    }

    /// <summary>
    /// Restart jump time and delay as well as cancelling Y velocity for consistent jumping.
    /// </summary>
    private void JumpSetup()
    {
        TimeSinceJump = 0;
        timeDelayToJump = JUMP_DELAY;
        rigidbody.velocity = new Vector3(rigidbody.velocity.x, 0, 0);
    }

    /// <summary>
    /// Makes the player jump
    /// </summary>
    private void Jump()
    {
        JumpSetup();
        canDoubleJump = false;
        rigidbody.AddForce(Vector3.up * jumpForce, ForceMode2D.Impulse);
        OnJump?.Invoke();
    }

    /// <summary>
    /// Starts pulling player down
    /// </summary>
    private void StopJumping(InputAction.CallbackContext _context)
    {
        if (rigidbody)
        {
            if (rigidbody.velocity.y > 0 && !isOnGround)
            {
                rigidbody.AddForce((1 - jumpCutMultiplier) * rigidbody.velocity.y * Vector3.down, ForceMode2D.Impulse);
            }
        }
    }

    /// <summary>
    /// Clamps vertical speed when falling
    /// </summary>
    private void ClampFallingSpeed()
    {
        if (rigidbody.velocity.y < maxFallingSpeed && !isInputDown)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, maxFallingSpeed);
        }
        else if(rigidbody.velocity.y < maxFallingSpeedDownward)
        {
            rigidbody.velocity = new Vector2(rigidbody.velocity.x, maxFallingSpeedDownward);
        }
    }

    /// <summary>
    /// Checks if the player is colliding with an object marked as jumpable
    /// </summary>
    private void IsPlayerOnGroundUpdate()
    {
        isOnGround = Physics2D.OverlapCircle(groundCheckPosition.position, groundCheckRadius, jumpableLayer);
        if (isOnGround)
        {
            if(isOnGround != isOnGroundLastFrame)
            {
                OnGroundedOnce?.Invoke();
            }

            timeSinceGrounded = coyoteTime;
            OnGrounded?.Invoke();
        }
        else
        {
            timeSinceGrounded -= Time.deltaTime;
        }

        isOnGroundLastFrame = isOnGround;
    }

    /// <summary>
    /// On Gizmos editor draw
    /// </summary>
    public void OnGizmos()
    {
        if(groundCheckPosition == null)
        {
            Debug.LogError("No ground check transform set in the player controller");
            return;
        }
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(groundCheckPosition.position, groundCheckRadius);
    }
}
