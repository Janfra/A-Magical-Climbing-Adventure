using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Windball Stats", menuName = "Scriptable Objects/New Windball Stats")]
public class SO_WindballStats : ScriptableObject
{
    [SerializeField]
    protected float movementPenaltyMultiplier = 0.8f;
    public float MovementPenaltyMultiplier => movementPenaltyMultiplier;

    [SerializeField]
    protected float minThrowForce = 1.5f;
    [SerializeField]
    protected float maxThrowForce = 3.0f;

    [SerializeField]
    protected float minPushForce = 1.5f;

    [SerializeField]
    protected float maxPushForce = 3.0f;

    [SerializeField]
    protected float velocityToTurnDoubleJump = 1.5f;
    public float VelocityToTurnDoubleJump => velocityToTurnDoubleJump;

    public float GetPushForce(float _chargeValue)
    {
        return Mathf.Lerp(minPushForce, maxPushForce, _chargeValue);
    }

    public float GetThrowForce(float _chageValue)
    {
        return Mathf.Lerp(minThrowForce, maxPushForce, _chageValue);
    }
}
