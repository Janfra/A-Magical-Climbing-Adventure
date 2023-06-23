using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class FallingPlatform : MonoBehaviour
{
    #region Variables & Constants

    [Header("Dependencies")]
    [SerializeField]
    private BoxCollider2D platformCollider;
    [SerializeField]
    private SpriteRenderer platformSprite;

    [Header("Configuration")]
    [SerializeField]
    private Timer stateChangingTimer;

    private int originalLayer;
    private int layerSwap = 7;
    private bool isFallingState;
    private const float TIME_OFF_PLATFORM_PERMITTED = 0.35f;
    private Timer cancelTimer = new(TIME_OFF_PLATFORM_PERMITTED);

    #endregion

    /// <summary>
    /// Init
    /// </summary>
    private void Awake()
    {
        if(platformCollider == null)
        {
            platformCollider = GetComponent<BoxCollider2D>();
        }

        if(platformSprite == null)
        {
            if(TryGetComponent(out SpriteRenderer renderer))
            {
                platformSprite = renderer;
            }
            else
            {
                Debug.LogError("There is no renderer in platform " + gameObject.name);
            }
        }
        originalLayer = gameObject.layer;
    }

    /// <summary>
    /// Start timer to make platform fall, stop cancelling timer if landing within allowed time
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (stateChangingTimer.IsTimerDone)
        {
            isFallingState = true;
            stateChangingTimer.SetTimerOnDoneAction(PlatformFall);
            stateChangingTimer.StartTimer(this);
        }
        cancelTimer.CancelTimerAndAction();
    }

    /// <summary>
    /// Start cancelling timer to instantly make platform fall
    /// </summary>
    /// <param name="collision"></param>
    private void OnCollisionExit2D(Collision2D collision)
    {
        cancelTimer.SetTimerOnDoneAction(InstantPlatformFall);
        cancelTimer.StartTimer(this);
    }

    /// <summary>
    /// Stop regen to avoid regenerating when player is in collider
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerEnter2D(Collider2D collision)
    {
        stateChangingTimer.PauseTimer(true);
    }

    /// <summary>
    /// Start regen again
    /// </summary>
    /// <param name="collision"></param>
    private void OnTriggerExit2D(Collider2D collision)
    {
        stateChangingTimer.PauseTimer(false);
    }

    /// <summary>
    /// Make platform fall, start regenerating
    /// </summary>
    private void PlatformFall()
    {
        gameObject.layer = layerSwap;
        isFallingState = false;
        platformCollider.isTrigger = true;
        platformSprite.color = Utils.SetColourOpacity(platformSprite.color, 0.5f);
        // Debug.Log($"Fell, timer time: {stateChangingTimer.CurrentTime}, target time: {stateChangingTimer.TargetTime}");
        StartCoroutine(DelayRegenerationStart());
    }

    /// <summary>
    /// Make platform regenerate and be interactable
    /// </summary>
    private void RegeneratePlatform()
    {
        gameObject.layer = originalLayer;
        platformCollider.isTrigger = false;
        platformSprite.color = Utils.SetColourOpacity(platformSprite.color, 1);
    }

    /// <summary>
    /// Make platform instantly fall
    /// </summary>
    private void InstantPlatformFall()
    {
        if (isFallingState)
        {
            stateChangingTimer.CancelTimer();
            Debug.Log("Made it instantly fall");
        }
    }

    /// <summary>
    /// Adter delay start regenerating. NOTE: Done like this to avoid overflow by starting tons of timers
    /// </summary>
    /// <returns></returns>
    private IEnumerator DelayRegenerationStart()
    {
        yield return new WaitForEndOfFrame();
        stateChangingTimer.StartTimer(this);
        stateChangingTimer.SetTimerOnDoneAction(RegeneratePlatform);
        yield return null;
    }
}
