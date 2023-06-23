using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_MagicAbilityBase : ScriptableObject
{
    public event Action OnAbilityUsed;
    public event Action<string> OnAbilityUsedWithName;

    [Header("Dependencies")]
    [SerializeField]
    protected MagicCasting caster;

    [Header("Configuration")]
    protected Timer abilityCooldownTimer;
    [SerializeField]
    protected SO_AbilityStats stats;

    /// <summary>
    /// Initializes, setting the caster owner
    /// </summary>
    /// <param name="_caster"></param>
    public virtual void Init(MagicCasting _caster)
    {
        caster = _caster;

        /// In case of NPC, add to on update
        //_caster.OnUpdateCast += OnTryUseAbility;
        abilityCooldownTimer = new(stats.AbilityCooldown);
    }

    public virtual void OnDisable() 
    {
        OnAbilityUsed = null;
    }

    /// <summary>
    /// Do ability logic
    /// </summary>
    public virtual void OnTryUseAbility()
    {
        // Example
        if (IsAbilityInCooldown())
        {
            return;
        }
    }

    /// <summary>
    /// Start cooldown and calls on ability used
    /// </summary>
    /// <param name="_caster"></param>
    protected void OnActivated(MagicCasting _caster)
    {
        abilityCooldownTimer.StartTimer(_caster);
        OnAbilityUsed?.Invoke();
        OnAbilityUsedWithName.Invoke(stats.SpellName);
    }

    /// <summary>
    /// Sets logic to determine if ability is on cooldown
    /// </summary>
    /// <returns></returns>
    public virtual bool IsAbilityInCooldown()
    {
        return !abilityCooldownTimer.IsTimerDone;
    }

    public override string ToString()
    {
        return stats.SpellName;
    }
}
