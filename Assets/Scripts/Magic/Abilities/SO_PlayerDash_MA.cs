using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Dash", menuName = "Scriptable Objects/New Player Dash Ability")]
public class SO_PlayerDash_MA : SO_MagicAbilityPlayer
{
    [SerializeField]
    SO_DashLogic dashLogic;
    private Timer dashTimer;

    public override void Init(MagicCasting _caster)
    {
        base.Init(_caster);
        dashTimer = new(dashLogic.DashDuration);
        PlayerMagicCasting player = (PlayerMagicCasting)_caster;
        if (player)
        {
            OnAbilityUsed += player.Dashed;
            dashTimer.SetTimerOnDoneAction(player.StopDashing);
        }
    }

    public override void OnTryUseAbility()
    {
        if (IsAbilityInCooldown())
        {
            return;
        }

        AttemptToCastDash();
    }

    protected void AttemptToCastDash()
    {
        if (caster.TryRemoveMana(stats.ManaCost))
        {
            Debug.Log("Dashed");
            caster.OnPhysicsCast += Dash;
        }
    }

    protected override void OnCast(InputAction.CallbackContext _context)
    {
        OnTryUseAbility();
    }

    protected override void UndoInputs()
    {
        base.UndoInputs();
        if(dashTimer != null)
        {
            dashTimer.CancelTimerAndAction();
        }
    }

    public override void SubscribeToEvent(AMagicalClimbingAdventure _inputs)
    {
        _inputs.Player.Dash.started += OnCast;
    }

    public override void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true)
    {
        _inputs.Player.Dash.started -= OnCast;
    }

    private void Dash()
    {
        caster.OnPhysicsCast -= Dash;
        OnActivated(caster);
        dashLogic.PerformDash(caster);
        dashTimer.StartTimer(caster);
    }
}
