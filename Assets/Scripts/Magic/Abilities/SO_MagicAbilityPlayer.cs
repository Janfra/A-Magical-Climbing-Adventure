using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class SO_MagicAbilityPlayer : SO_MagicAbilityBase, IInputListener
{
    public override void OnDisable()
    {
        base.OnDisable();
        UndoInputs();
    }

    protected virtual void UndoInputs()
    {
        if(GameManager.Instance != null)
        {
            GameManager.Instance.inputManager.UnsubscribeToPlayerInput(this);
        }
    }

    protected virtual void SetupInput()
    {
        IInputListener.AttemptToSubscribe(this);
    }

    protected abstract void OnCast(InputAction.CallbackContext _context);
     
    public override void Init(MagicCasting _caster)
    {
        base.Init(_caster);
        SetupInput();
    }

    public abstract void SubscribeToEvent(AMagicalClimbingAdventure _inputs);

    public abstract void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true);
}
