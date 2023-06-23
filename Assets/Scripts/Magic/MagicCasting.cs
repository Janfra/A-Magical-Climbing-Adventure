using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicCasting : MonoBehaviour
{
    public Action OnUpdateCast;
    public Action OnPhysicsCast;

    [Header("Dependencies")]
    [SerializeField]
    protected Rigidbody2D rigidBody;
    public Rigidbody2D RigidBody => rigidBody;

    // For debug and no duplicates
    protected List<SO_MagicAbilityBase> magicAbilities = new();
    protected Vector2 movementDirection;
    public Vector2 MovementDirection => movementDirection;
    public Vector2 MagicDirection { get; protected set; }
    protected float manaPool;
    public float CurrentMana => manaPool;

    [Header("Configuration")]
    [SerializeField]
    private int maxMana = 2;
    public int MaxMana => maxMana;

    [SerializeField]
    protected float manaRegenerationPerSecond = 1;

    private void Start()
    {
        manaPool = maxMana;
    }

    protected virtual void Update()
    {
        // For now, may instead add a way to check for individual inputs that then call their respective ability via a dictionary
        if(manaPool > 0)
        {
            OnUpdateCast?.Invoke();
        }
    }

    protected virtual void OnDisable()
    {
        OnUpdateCast = null;
        OnPhysicsCast = null;
        foreach (var magic in magicAbilities)
        {
            magic.OnDisable();
        }
    }

    protected virtual void FixedUpdate()
    {
        OnPhysicsCast?.Invoke();
    }

    public virtual void AddAbility(SO_MagicAbilityBase _ability)
    {
        if (!magicAbilities.Contains(_ability))
        {
            magicAbilities.Add(_ability);
            _ability.Init(this);
            Debug.Log($"New magic ability: {_ability}!");
        }
    }

    /// <summary>
    /// Attempts to remove the mana cost of an ability
    /// </summary>
    /// <param name="_manaCost"></param>
    /// <returns></returns>
    public virtual bool TryRemoveMana(int _manaCost)
    {
        if(manaPool >= _manaCost)
        {
            manaPool -= _manaCost;
            return true;
        }
        return false;
    }

    protected virtual void TryRegenerateMana()
    {
        if (manaPool < maxMana)
        {
            manaPool += manaRegenerationPerSecond * Time.deltaTime;
            manaPool = Mathf.Clamp(manaPool, 0, maxMana);
        }
    }
}
