using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[CreateAssetMenu(fileName = "New Windball", menuName = "Scriptable Objects/New Player Windball Ability")]
public class SO_PlayerWindball_MA : SO_MagicAbilityPlayer
{
    [SerializeField]
    private SO_WindballLogic windballLogic;
    private float chargedValue;
    private bool isValid = false;
    private PlayerMagicCasting playerCaster;

    public override void Init(MagicCasting _caster)
    {
        base.Init(_caster);
        playerCaster = (PlayerMagicCasting)_caster;
    }

    public override void OnTryUseAbility()
    {
        if (IsAbilityInCooldown())
        {
            isValid = false;
            return;
        }

        if (caster.TryRemoveMana(stats.ManaCost))
        {
            Debug.Log("Casted windball");
            isValid = true;
            playerCaster.StartCastAnimation();
            playerCaster.SetMovementSpeedPenalty(windballLogic.MovementPenaltyMultiplier);
            caster.StartCoroutine(ChargeWindball());
        }
        else
        {
            Debug.Log("Not enough mana");
            isValid = false;
        }
    }

    protected override void OnCast(InputAction.CallbackContext _context)
    {
        OnTryUseAbility();
    }

    public override void SubscribeToEvent(AMagicalClimbingAdventure _inputs)
    {
        _inputs.Player.Fire.started += OnCast;
        _inputs.Player.Fire.canceled += OnReleaseWindball;
    }

    public override void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true)
    {
        _inputs.Player.Fire.started -= OnCast;
        _inputs.Player.Fire.canceled -= OnReleaseWindball;
    }

    private IEnumerator ChargeWindball()
    {
        yield return null;
        while (isValid)
        {
            chargedValue += Time.deltaTime;
            yield return null;
        }
    }

    private void OnReleaseWindball(InputAction.CallbackContext _context)
    {
        if (isValid)
        {
            isValid = false;
            chargedValue = Mathf.Clamp01(chargedValue);
            // Since there is a cooldown it doesnt matter is not in the same frame
            caster.OnPhysicsCast += ShootWindball;
            playerCaster.SetMovementSpeedPenalty(1);
            playerCaster.StopCastAnimation();
        }
    }

    private void ShootWindball()
    {
        caster.OnPhysicsCast -= ShootWindball;
        windballLogic.ThrowWindball(caster, chargedValue);
        OnActivated(caster);
        chargedValue = 0.0f;
    }
}
