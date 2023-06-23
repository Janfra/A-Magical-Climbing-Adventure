using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Rigidbody2D))]
public class PlayerController : MonoBehaviour, IInputListener
{
    #region Variables

    [SerializeField]
    public bool disableInputs = false;

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody2D playerRigidbody;
    public Rigidbody2D PlayerRigidbody => playerRigidbody;

    [Header("Modules")]
    /// <summary>
    /// Handles Jumping logic
    /// </summary>
    [SerializeField]
    private PlayerJump jumpHandler;
    public PlayerJump PlayerJump => jumpHandler;

    /// <summary>
    /// Handles movement, aka horizontal movement
    /// </summary>
    [SerializeField]
    private PlayerMovement movementHandler;
    public PlayerMovement MovementHandler => movementHandler;


    #endregion

    /// <summary>
    /// Initializing
    /// </summary>
    private void Awake()
    {
        if(playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
    }

    private void Start()
    {
        IInputListener.AttemptToSubscribe(this);
    }

    private void OnEnable()
    {
        if (playerRigidbody == null)
        {
            playerRigidbody = GetComponent<Rigidbody2D>();
        }
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    void Update()
    {
        if (disableInputs)
        {
            return;
        }

        jumpHandler.OnUpdate();
        movementHandler.OnUpdate();
    }

    /// <summary>
    /// Update for physics based
    /// </summary>
    private void FixedUpdate()
    {
        if (disableInputs)
        {
            return;
        }

        jumpHandler.HandleJump();
        movementHandler.HandleMovement();
    }

    /// <summary>
    /// Draws gizmos on editor
    /// </summary>
    private void OnDrawGizmos()
    {
        jumpHandler.OnGizmos();
    }

    public void SubscribeToEvent(AMagicalClimbingAdventure _inputs)
    {
        jumpHandler.Init(this, _inputs);
        movementHandler.Init(this, _inputs);
    }

    public void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true)
    {
        jumpHandler.UndoInputBinding(_inputs, _isTemporary);
        movementHandler.UndoInputBinding(_inputs, _isTemporary);
    }
}

public abstract class PlayerControllerModule
{
    public abstract void Init(PlayerController _controller, AMagicalClimbingAdventure _inputs);
    public abstract void UndoInputBinding(AMagicalClimbingAdventure _inputs, bool _isTemporary);
    public abstract void OnUpdate();
}