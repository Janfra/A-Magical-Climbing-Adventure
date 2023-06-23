using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class MovingPlatform : MonoBehaviour
{
    #region Variables

    [Header("Dependencies")]
    [SerializeField]
    private Rigidbody2D platformRigidbody;

    [Header("Configuration")]
    [SerializeField]
    private Vector2 pointB;
    [SerializeField]
    private float durationToArrive;

    // Speed calculation for velocity
    private Vector2 pointA;
    private float distance;
    private float speed;

    // Direction setting
    private Vector3 startPosition;
    private float time;
    private bool isMovingToPointB;
    private bool isPaused;

    // Calculating velocity for colliding objects
    private List<Rigidbody2D> carriedObjects = new();

    // Asuming there is 1 player
    private static Rigidbody2D playerRigidbody;
    private static PlayerMovement playerMovement;
    private static bool isPlayerMoving;

    #endregion

    /// <summary>
    /// Init
    /// </summary>
    private void Awake()
    {
        if(platformRigidbody == null)
        {
            platformRigidbody = GetComponent<Rigidbody2D>();
        }
        pointA = transform.position;
        pointB += new Vector2(transform.position.x, transform.position.y);
        startPosition = pointA;
        isMovingToPointB = true;
        isPaused = false;

        distance = (pointA - pointB).magnitude;
        speed = distance / durationToArrive;
    }

    /// <summary>
    /// Move platform if not paused
    /// </summary>
    private void FixedUpdate()
    {
        if (isPaused)
        {
            return;
        }

        if (isMovingToPointB)
        {
            if (MoveToPositionWithVelocity(pointB))
            {
                startPosition = pointB;
                time = 0.0f;
                isMovingToPointB = false;
            }
        }
        else
        {
            if (MoveToPositionWithVelocity(pointA))
            {
                startPosition = pointA;
                time = 0.0f;
                isMovingToPointB = true;
            }
        }
    }

    /// <summary>
    /// Add collision rigidbody to move them
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.TryGetComponent(out Rigidbody2D collisionRigidbody))
        {
            if (collision.gameObject.TryGetComponent(out PlayerController player))
            {
                playerRigidbody = collisionRigidbody;
                playerMovement = player.MovementHandler;
                playerMovement.OnMove += CancelAffectedSource;
            }
            else if(collisionRigidbody.bodyType != RigidbodyType2D.Static)
            {
                carriedObjects.Add(collisionRigidbody);
            }
        }
    }

    /// <summary>
    /// Match platforms velocity on the objects to ensure they move along with the platform
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionStay2D(Collision2D collision)
    {
        foreach (Rigidbody2D collisionRigidbody in carriedObjects)
        {
            if (Mathf.Abs(collisionRigidbody.velocity.x) <= Mathf.Abs(platformRigidbody.velocity.x))
            {
                collisionRigidbody.velocity = new Vector2(platformRigidbody.velocity.x, collisionRigidbody.velocity.y);
            }
        }

        // Is one of the objects a player?
        if (playerMovement != null)
        {
            playerMovement.isAffectedBySource = true;

            if (Mathf.Abs(playerRigidbody.velocity.x) <= Mathf.Abs(platformRigidbody.velocity.x) && !isPlayerMoving)
            {
                playerRigidbody.velocity = new Vector2(platformRigidbody.velocity.x, playerRigidbody.velocity.y);
            }
            else
            {
                playerMovement.isAffectedBySource = false;
            }
        }
    }

    /// <summary>
    /// If an object leaves the platform, stop matching velocity
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        if (collision.gameObject.TryGetComponent(out Rigidbody2D collisionRigidbody))
        {
            if (collision.gameObject.TryGetComponent(out PlayerController player))
            {
                playerMovement.isAffectedBySource = false;
                playerRigidbody = null;
                playerMovement = null;
            }
            else
            {
                carriedObjects.Remove(collisionRigidbody);
            }
        }
    }

    /// <summary>
    /// Select position to move to using velocity precalculated at awake
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    private bool MoveToPositionWithVelocity(Vector3 _position)
    {
        // Vector3 newPosition = new Vector3(_position.x, _position.y) - transform.position;
        time += Time.fixedDeltaTime;
        float progress = Mathf.Clamp01(time / durationToArrive);
        Vector3 direction = _position - startPosition;
        direction.Normalize();

        platformRigidbody.velocity = direction * speed;

        return progress == 1;
    }

    /// <summary>
    /// Stop affecting players velocity if input is given
    /// </summary>
    /// <param name="_isMoving"></param>
    private static void CancelAffectedSource(bool _isMoving)
    {
        isPlayerMoving = _isMoving;
    }

    /// <summary>
    /// Show moving points in editor
    /// </summary>
    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 0.1f);
        Gizmos.DrawSphere(pointB + new Vector2(transform.position.x, transform.position.y), 0.1f);
    }
}

#region OUTDATED

///// <summary>
///// Select position to move to
///// </summary>
///// <param name="_position"></param>
///// <returns></returns>
//private bool MoveToPosition(Vector2 _position)
//{
//    // Vector3 newPosition = new Vector3(_position.x, _position.y) - transform.position;
//    time += Time.fixedDeltaTime;
//    float progress = Mathf.Clamp01(time / durationToArrive);
//    Vector3 currentPosition = Vector3.Lerp(startPosition, _position, progress);
//    platformRigidbody.MovePosition(currentPosition);

//    return progress == 1;
//}

#endregion
