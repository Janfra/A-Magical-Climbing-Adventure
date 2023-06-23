using System;
using UnityEngine;
using UnityEngine.InputSystem;

[Serializable]
public class PlayerMovement : PlayerControllerModule
{
    /// <summary>
    /// Call when the x velocity is not 0, returns true if input is given or false when stopping
    /// </summary>
    public event Action<bool> OnMove;

    #region Variables & Constants

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody2D rigidbody;

    #region Movement Direction

    // Movement direction
    private float xInputDirection;

    // 1 to -1 representing if moving left or right
    private float horizontalMovementDirection;
    private int lastInputDirection;
    public Vector2 MovementDirection { get; private set; }

    #endregion

    #region Slowing Down

    private float slideSlowdownTime;
    /// <summary>
    /// Is the movement being affected by an outside source, if so dont cancel movement speed
    /// </summary>
    public bool isAffectedBySource = false;

    private float movementSpeedPenalty = 1.0f;

    #endregion

    [Header("Configuration")]
    [SerializeField]
    private float acceleration = 2.0f;
    [SerializeField]
    private float timeToStopOnSlide = 1.3f;
    [SerializeField]
    private float maxMovementSpeed = 2.0f;
    [SerializeField]
    private float maxGeneralSpeed = 10.0f;
    [SerializeField]
    private float turningSlowDownMultiplier = 0.8f;

    [Header("For checking")]
    private float speedBeingAdded;

    #endregion

    /// <summary>
    /// Initializes the class
    /// </summary>
    /// <param name="_controller"></param>
    public override void Init(PlayerController _controller, AMagicalClimbingAdventure _inputs)
    {
        rigidbody = _controller.PlayerRigidbody;
        _inputs.Player.Move.performed += UpdateInputs;
        _inputs.Player.Move.canceled += UpdateInputs;
    }

    /// <summary>
    /// Runs on update to update logic variables
    /// </summary>
    public override void OnUpdate()
    {
        UpdateMovementDirectionAndSlideTime();
        ClampMovementSpeed();
    }

    /// <summary>
    /// Handles movement logic on fixed update
    /// </summary>
    public void HandleMovement()
    {
        if(xInputDirection == 0 && !isAffectedBySource)
        {
            SlowDownSpeed();
        }
        else
        {
            slideSlowdownTime = 0.0f;
            CheckForMovingOppositeDirection();
            MoveToInputDirection();
        }
    }

    /// <summary>
    /// Clears events
    /// </summary>
    public override void UndoInputBinding(AMagicalClimbingAdventure _inputs, bool _isTemporary)
    {
        if (!_isTemporary)
        {
            OnMove = null;
        }

        MovementDirection = new();
        xInputDirection = 0.0f;
        _inputs.Player.Move.performed -= UpdateInputs;
        _inputs.Player.Move.canceled -= UpdateInputs;
    }

    /// <summary>
    /// Sets the movement speed penalty on movement
    /// </summary>
    /// <param name="_penaltyMultiplier">0 to 1 value for penalty</param>
    public void SetMovementSpeedPenalty(float _penaltyMultiplier)
    {
        movementSpeedPenalty = Mathf.Clamp01(_penaltyMultiplier);
    }

    /// <summary>
    /// Checks for player input to move sideways or stop
    /// </summary>
    private void UpdateInputs(InputAction.CallbackContext _context)
    {
        xInputDirection = _context.ReadValue<Vector2>().x;
        
        if(xInputDirection > 0)
        {
            xInputDirection = 1;
        }
        else if(xInputDirection < 0)
        {
            xInputDirection = -1;
        }
        else
        {
            xInputDirection = 0;
        }

        MovementDirection = _context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Moves player towards input direction
    /// </summary>
    private void MoveToInputDirection()
    {
        float speedToAdd = (xInputDirection * acceleration * Time.fixedDeltaTime) * movementSpeedPenalty;
        if(speedToAdd != 0)
        {
            OnMove?.Invoke(true);
            speedToAdd += rigidbody.velocity.x;
            speedBeingAdded = speedToAdd;
            if(Mathf.Abs(speedToAdd) < maxMovementSpeed)
            {
                rigidbody.velocity = new(Mathf.Clamp(speedToAdd, -maxMovementSpeed, maxMovementSpeed), rigidbody.velocity.y);
            }
        }
        // Debug.Log($"Adding speed of: {speedToAdd}, velocity result: {currentVelocity + speedToAdd}");
    }

    /// <summary>
    /// Checks if the player is moving in the opposite direction to quickly slow down
    /// </summary>
    private void CheckForMovingOppositeDirection()
    {
        // If returns -1, is pointing towards the other direction
        bool isOppositeDirection = Vector2.Dot(new(horizontalMovementDirection, 0), new(xInputDirection, 0)) == -1 ? true : false;
        if(isOppositeDirection && rigidbody.velocity.x != 0)
        {
            float newVelocity = rigidbody.velocity.x * turningSlowDownMultiplier;
            rigidbody.velocity = new(newVelocity, rigidbody.velocity.y);

            // Debug.Log("Moving opposite direction");
        }
    }

    /// <summary>
    /// Updates the current speed and direction of movement
    /// </summary>
    private void UpdateMovementDirectionAndSlideTime()
    {
        // If moving at all
        if(rigidbody.velocity.SqrMagnitude() != 0)
        {
            float rightDotProduct = Vector2.Dot(rigidbody.velocity.normalized, Vector2.right);

            // sets it to 1, 0, -1
            horizontalMovementDirection = Mathf.Sign(rightDotProduct);

        }
        else
        {
            // Set to 0, aka nothing
            horizontalMovementDirection = new();
        }

        // Update to last input direction
        if(xInputDirection != 0)
        {
            lastInputDirection = (int)xInputDirection;
        }

        if(MovementDirection.SqrMagnitude() == 0)
        {
            MovementDirection = new(lastInputDirection, 0);
        }

        slideSlowdownTime += Time.deltaTime;
    }

    /// <summary>
    /// Slows down the player until stopping
    /// </summary>
    private void SlowDownSpeed()
    {
        if (rigidbody.velocity.x != 0 && !isAffectedBySource)
        {
            OnMove?.Invoke(false);
            float velocityToSet = Mathf.Lerp(rigidbody.velocity.x, 0, Mathf.Clamp01(slideSlowdownTime / timeToStopOnSlide));
            rigidbody.velocity = new(velocityToSet, rigidbody.velocity.y);

            // float velocityToSet = rigidbody.velocity.x - (horizontalMovementDirection * slowDownMultiplier * Time.fixedDeltaTime);
            // Debug.Log($"Velocity being slowed to {velocityToSet} from {rigidbody.velocity.x}");
        }
    }

    /// <summary>
    /// Clamps horizontal movement speed
    /// </summary>
    private void ClampMovementSpeed()
    {
        if(Mathf.Abs(rigidbody.velocity.x) > maxGeneralSpeed)
        {
            rigidbody.velocity = new(Mathf.Clamp(rigidbody.velocity.x, -maxGeneralSpeed, maxGeneralSpeed), rigidbody.velocity.y);
        }
    }
}
