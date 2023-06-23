using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class AirCurrent : MonoBehaviour
{
    [Header("Config")]
    [SerializeField]
    private Vector2 pushingDirection = Vector2.up;
    [SerializeField]
    private float pushingForce = 1.0f;
    [SerializeField]
    private float maxPushingSpeed = 5.0f;

    private void OnValidate()
    {
        pushingDirection.Normalize();
    }

    private void Awake()
    {
        GetComponent<BoxCollider2D>().isTrigger = true;
    }

    private void OnTriggerStay2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out Rigidbody2D collisionRigidbody))
        {
            Vector2 currentVelocity = collisionRigidbody.velocity;
            Vector2 newVelocity = pushingDirection.normalized * pushingForce;

            if(newVelocity.x + currentVelocity.x <= maxPushingSpeed)
            {
                collisionRigidbody.velocity += new Vector2(newVelocity.x, 0.0f);
            }

            if (newVelocity.y + currentVelocity.y <= maxPushingSpeed)
            {
                collisionRigidbody.velocity += new Vector2(0.0f, newVelocity.y);
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + new Vector3(pushingDirection.x, pushingDirection.y) * 2.0f);
    }
}
