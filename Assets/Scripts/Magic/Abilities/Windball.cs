using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Windball : MonoBehaviour, ObjectPooler.IPoolGetComponent
{
    protected Action<Collider2D> currentState;
    public int Index { get; set; }

    [SerializeField]
    protected Rigidbody2D rb;
    protected SO_WindballStats stats;
    protected bool wasTargetHit = false;
    protected float chargedValue;


    private void OnEnable()
    {
        wasTargetHit = false;
        currentState = CheckForPushing;
        StartCoroutine(CheckForVelocity());
    }

    protected virtual void OnDisable()
    {
        StopAllCoroutines();
        ObjectPooler.IPoolGetComponent.SetToTopOfList(ObjectPooler.PoolObjName.Windball, this);
    }

    public void Shoot(Vector2 _shootDirection, SO_WindballStats _stats, float _chargedValue)
    {
        stats = _stats;
        rb.AddForce(_shootDirection * stats.GetThrowForce(_chargedValue), ForceMode2D.Impulse);
        chargedValue = _chargedValue;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        currentState?.Invoke(collision);
    }

    protected virtual void CheckForPushing(Collider2D _collision)
    {
        if(_collision.TryGetComponent(out AirCurrent airCurrent))
        {
            return;
        }

        wasTargetHit = true;
        if (_collision.TryGetComponent(out Rigidbody2D targetRigidbody))
        {
            targetRigidbody.velocity += rb.velocity.normalized * stats.GetPushForce(chargedValue);
        }
        gameObject.SetActive(false);
    }

    protected virtual void CheckForDoubleJump(Collider2D _collision)
    {
        if (_collision.TryGetComponent(out AirCurrent airCurrent))
        {
            return;
        }

        if (_collision.TryGetComponent(out PlayerController player))
        {
            player.PlayerJump.SetDoubleJump(true);
        }
        gameObject.SetActive(false);
    }

    private IEnumerator CheckForVelocity()
    {
        yield return null;
        if(rb.velocity.sqrMagnitude != 0)
        {
            while (!wasTargetHit && rb.velocity.sqrMagnitude > stats.VelocityToTurnDoubleJump)
            {
                // Debug.Log($" Windball velocity sqrLenght: {rb.velocity.sqrMagnitude}");
                yield return null;
            }
        }

        // Debug.Log($" Windball velocity sqrLenght: {rb.velocity.sqrMagnitude} at time of stopping");
        if (!wasTargetHit)
        {
            Debug.Log("Checking for player!");
            currentState = CheckForDoubleJump;
        }
    }
}
