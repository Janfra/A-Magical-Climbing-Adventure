using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class SO_DashNPC_MA : SO_MagicAbilityBase
{
    SO_DashLogic dashLogic;

    public override void OnTryUseAbility()
    {
        if (!IsAbilityInCooldown())
        {
            return;
        }

        AttemptToCastDash();
    }

    protected void AttemptToCastDash()
    {
        if (caster.TryRemoveMana(stats.ManaCost))
        {
            caster.OnPhysicsCast += Dash;
        }
    }

    private void Dash()
    {
        caster.OnPhysicsCast -= Dash;
        OnActivated(caster);
        dashLogic.PerformDash(caster);
    }
}
