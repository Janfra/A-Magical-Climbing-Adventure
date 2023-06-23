using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Dash Stats", menuName = "Scriptable Objects/New Dash Force")]
public class SO_DashLogic : ScriptableObject
{
    [SerializeField]
    protected float dashForce = 4.0f;
    [SerializeField]
    protected float dashDuration = 0.6f;
    public float DashDuration => dashDuration;

    public void PerformDash(MagicCasting _caster)
    {
        Vector2 dashDirection = _caster.MagicDirection.normalized;

        // If not going same direction, reset the velocity to have consistent minimum velocity given by dash
        Vector2 upDirection = new(0, _caster.RigidBody.velocity.y);
        Vector2 sideDirection = new(_caster.RigidBody.velocity.x, 0);

        // Debug.Log($"Initial velocity: {_caster.RigidBody.velocity}");

        // Going same up direction?
        if (Vector2.Dot(upDirection, dashDirection) < 0)
        {
            _caster.RigidBody.velocity = new(_caster.RigidBody.velocity.x, 0);
        }

        // Going same side direction?
        if(Vector2.Dot(sideDirection, dashDirection) < 0)
        {
            _caster.RigidBody.velocity = new(0, _caster.RigidBody.velocity.y);
        }

        _caster.RigidBody.AddForce(dashDirection * dashForce, ForceMode2D.Impulse);
        // Debug.Log($"result velocity: {_caster.RigidBody.velocity}");
        // Debug.Log($"Dashed at direction: {dashDirection}");
    }
}