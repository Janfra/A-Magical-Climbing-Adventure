using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class RegenerateOnTimer : MonoBehaviour
{
    public event Action OnDissappeared;
    public event Action OnRegenerated;

    [Header("Dependencies")]
    [SerializeField]
    protected Collider2D platformCollider;
    [SerializeField]
    protected SpriteRenderer platformSprite;
    protected Timer stateChangingTimer;

    [Header("Configuration")]
    [SerializeField]
    protected float timeToDissapear;
    [SerializeField]
    protected float timeToRegen;

    [SerializeField]
    protected int layerIndexSwap = 7;
    protected int originalLayer;

    private void Awake()
    {
        if (platformCollider == null)
        {
            platformCollider = GetComponent<BoxCollider2D>();
        }

        if (platformSprite == null)
        {
            if (TryGetComponent(out SpriteRenderer renderer))
            {
                platformSprite = renderer;
            }
            else
            {
                Debug.LogError("There is no renderer in platform " + gameObject.name);
            }
        }
        originalLayer = gameObject.layer;
        stateChangingTimer = new(timeToDissapear);
    }

    protected virtual void OnCollisionEnter2D(Collision2D collision)
    {
        TryDissapear();
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        TryDissapear();
    }

    private void TryDissapear()
    {
        if (stateChangingTimer.IsTimerDone)
        {
            stateChangingTimer.SetTimerOnDoneAction(Dissapear);
            stateChangingTimer.StartTimer(this);
        }
    }

    private void Dissapear()
    {
        OnDissappeared?.Invoke();
        stateChangingTimer.SetTimer(timeToRegen);
        gameObject.layer = layerIndexSwap;
        platformSprite.color = Utils.SetColourOpacity(platformSprite.color, 0.5f);
        Debug.Log($"Fell, timer time: {stateChangingTimer.CurrentTime}, target time: {stateChangingTimer.TargetTime}, gameobject: {gameObject.name}");
        StartCoroutine(DelayRegenerationStart());
    }

    private void RegenerateObject()
    {
        OnRegenerated?.Invoke();
        stateChangingTimer.SetTimer(timeToDissapear);
        gameObject.layer = originalLayer;
        platformSprite.color = Utils.SetColourOpacity(platformSprite.color, 1);
    }

    private IEnumerator DelayRegenerationStart()
    {
        yield return new WaitForEndOfFrame();
        stateChangingTimer.StartTimer(this);
        stateChangingTimer.SetTimerOnDoneAction(RegenerateObject);
        yield return null;
    }
}
