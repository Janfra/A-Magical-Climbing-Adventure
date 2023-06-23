using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(RegenerateOnTimer))]
public class RegenerativeWindball : Windball 
{
    private bool isActive;

    private void Awake()
    {
        RegenerateOnTimer timer = GetComponent<RegenerateOnTimer>();
        timer.OnDissappeared += DisableWindball;
        timer.OnRegenerated += EnableWindball;
    }

    protected override void OnDisable()
    {
        StopAllCoroutines();
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        currentState.Invoke(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        currentState.Invoke(collision);
    }

    private void DisableWindball()
    {
        isActive = false;
    }

    private void EnableWindball()
    {
        isActive = true;
    }

    protected override void CheckForPushing(Collider2D _collision)
    {
        if (!isActive) return;
        if (_collision.TryGetComponent(out AirCurrent airCurrent))
        {
            return;
        }

        wasTargetHit = true;
        if (_collision.TryGetComponent(out Rigidbody2D targetRigidbody))
        {
            targetRigidbody.velocity += rb.velocity.normalized * stats.GetPushForce(chargedValue);
        }
    }

    protected override void CheckForDoubleJump(Collider2D _collision)
    {
        if (!isActive) return;
        if (_collision.TryGetComponent(out AirCurrent airCurrent))
        {
            return;
        }

        if (_collision.TryGetComponent(out PlayerController player))
        {
            player.PlayerJump.SetDoubleJump(true);
        }
    }
}
