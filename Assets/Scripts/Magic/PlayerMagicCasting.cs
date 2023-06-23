using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(PlayerController))]
public class PlayerMagicCasting : MagicCasting, IInputListener
{
    public event Action<float, float> OnManaUpdate;
    public event Action<bool> OnCastInProgress;

    private bool isDashing;
    public bool IsDashingState => isDashing;
    public float magicCastRadius = 0.5f;

    [SerializeField]
    private PlayerController controller;
    [SerializeField]
    private Transform magicPosition;

    private void Awake()
    {
        if(controller == null)
        {
            controller = GetComponent<PlayerController>();
        }
    }

    private void Start()
    {
        if (IInputListener.AttemptToSubscribe(this))
        {
            // On controller change subscribe if need to
        }
    }

    protected override void OnDisable()
    {
        base.OnDisable();
    }

    // NOTE: Will redo this
    private void SetMagicPosition(InputAction.CallbackContext _context)
    {
        // If is not gamepad, convert mouse pos to world pos
        if (!GameManager.Instance.inputManager.IsGamepad)
        {
            Vector2 position = _context.ReadValue<Vector2>();
            Vector2 worldPos = Camera.main.ScreenToWorldPoint(position);
            Vector2 playerPos = new Vector2(transform.position.x, transform.position.y);

            MagicDirection = worldPos - playerPos;
            MagicDirection = MagicDirection.normalized * magicCastRadius;
            magicPosition.position = MagicDirection + playerPos;

            return;
        }

        // NOT TESTED
        magicPosition.position = controller.MovementHandler.MovementDirection + new Vector2(transform.position.x, transform.position.y);
        MagicDirection = controller.MovementHandler.MovementDirection;
    }

    public override void AddAbility(SO_MagicAbilityBase _ability)
    {
        base.AddAbility(_ability);
        _ability.OnAbilityUsedWithName += AbilityCasted;
    }

    private void AbilityCasted(string _name)
    {
        // Gets the ability name of the current ability used
    }

    public void StartCastAnimation()
    {
        OnCastInProgress?.Invoke(true);
    }

    public void StopCastAnimation()
    {
        OnCastInProgress?.Invoke(false);
    }

    public void Dashed()
    {
        isDashing = true;
        AudioManager.Instance.PlayAudio("Dash");
    }

    public void StopDashing()
    {
        isDashing = false;
        AudioManager.Instance.StopAudio("Dash");
        Debug.Log("Stopped dashing");
    }

    public void SetMovementSpeedPenalty(float _penaltyMultiplier)
    {
        // Could change the grab the lowest penalty from a list and remove them as they expire until there is no penalty so = 1
        controller.MovementHandler.SetMovementSpeedPenalty(_penaltyMultiplier);
    }

    protected override void Update()
    {
        movementDirection = controller.MovementHandler.MovementDirection;
        TryRegenerateMana();
    }

    public override bool TryRemoveMana(int _manaCost)
    {
        if (manaPool >= _manaCost)
        {
            OnManaUpdate?.Invoke(manaPool - _manaCost, manaPool);
            manaPool -= _manaCost;
            return true;
        }
        return false;
    }

    protected override void TryRegenerateMana()
    {
        if (manaPool < MaxMana)
        {
            float valueToAdd = manaRegenerationPerSecond * Time.deltaTime;
            manaPool += valueToAdd;
            manaPool = Mathf.Clamp(manaPool, 0, MaxMana);
            OnManaUpdate?.Invoke(manaPool, manaPool - valueToAdd);
        }
    }

    public void SubscribeToEvent(AMagicalClimbingAdventure _inputs)
    {
        _inputs.Player.Look.performed += SetMagicPosition;
        _inputs.Player.Look.canceled += SetMagicPosition;
        _inputs.Player.Look.started += SetMagicPosition;
    }

    public void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true)
    {
        _inputs.Player.Look.performed -= SetMagicPosition;
        _inputs.Player.Look.canceled -= SetMagicPosition;
        _inputs.Player.Look.started -= SetMagicPosition;
    }
}
