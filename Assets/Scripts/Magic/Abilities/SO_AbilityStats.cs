using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ability Stats", menuName ="Scriptable Objects/New Ability Stats")]
public class SO_AbilityStats : ScriptableObject
{
    [SerializeField]
    protected string spellName = "Unknown Spell";
    public string SpellName => spellName;

    [SerializeField]
    protected int manaCost = 1;
    public int ManaCost => manaCost;

    [SerializeField]
    protected float abilityCooldown = 1.0f;
    public float AbilityCooldown => abilityCooldown;
}
