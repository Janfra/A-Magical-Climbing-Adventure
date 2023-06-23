using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicBook : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private SO_MagicAbilityPlayer abilityGranted;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out MagicCasting caster))
        {
            caster.AddAbility(abilityGranted);
        }
    }
}
