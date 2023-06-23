using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class RespawnPoint : MonoBehaviour
{
    private static RespawnPoint currentRespawnPoint;
    private static Transform playerTransform;
    private static Vector2 startPosition;
    private static SpriteRenderer playerRenderer;
    private static Timer delayTimer = new(RESPAWN_DELAY);
    private const float RESPAWN_DELAY = 0.6f;

    [Header("Dependencies")]
    [SerializeField]
    private BoxCollider2D boxCollider;

    [Header("Configuration")]
    [SerializeField]
    private Vector2 respawnPosition;

    private void Awake()
    {
        if(boxCollider == null)
        {
            boxCollider = GetComponent<BoxCollider2D>();
        }
        boxCollider.isTrigger = true;
        respawnPosition += new Vector2(transform.position.x, transform.position.y);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out Transform transform) && collision.TryGetComponent(out SpriteRenderer renderer))
        {
            playerRenderer = renderer;
            playerTransform = transform;
            currentRespawnPoint = this;
        }
    }

    public static void InstantlyRespawnPlayer()
    {
        if (!AreDependenciesMissing())
        {
            MovePlayerToRespawnPosition();
        }
    }

    public static void RespawnPlayerAfterDelay(bool _isFrozenInPlace = false)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        delayTimer.SetTimerOnDoneAction(MovePlayerToRespawnPosition);
        delayTimer.StartTimer(currentRespawnPoint);
        if (_isFrozenInPlace)
        {
            currentRespawnPoint.StartCoroutine(WhileOnTimer(FreezePlayerPosition));
        }
        else
        {
            currentRespawnPoint.StartCoroutine(WhileOnTimer(MoveSlowlyBack));
        }
    }

    private static void MovePlayerToRespawnPosition()
    {
        playerTransform.position = currentRespawnPoint.respawnPosition;
    }

    private static void FreezePlayerPosition()
    {
        playerTransform.position = startPosition;
    }

    private static void MoveSlowlyBack()
    {
        Vector2 currentPosition = new Vector2(playerTransform.position.x, playerTransform.position.y);
        if (currentPosition == startPosition)
        {
            return;
        }
        Vector2 direction = currentPosition - startPosition;
        playerTransform.position = currentPosition + direction.normalized * 2f * Time.deltaTime;
    }

    private static IEnumerator WhileOnTimer(Action _action)
    {
        while(delayTimer.GetTimeNormalized() != 1)
        {
            playerRenderer.material.SetFloat("_DissolveAmount", Mathf.Lerp(0.75f, 0, delayTimer.GetTimeNormalized()));
            _action.Invoke();
            yield return null;
        }

        playerRenderer.material.SetFloat("_DissolveAmount", 1);
    }

    private static bool AreDependenciesMissing()
    {
        if (playerTransform != null && currentRespawnPoint != null)
        {
            startPosition = playerTransform.position;
            return false;
        }
        else
        {
            Debug.LogError("No respawn position set on player!");
            return true;
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(respawnPosition + new Vector2(transform.position.x, transform.position.y), 0.5f);
    }
}
