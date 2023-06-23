using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class DashWall : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    private BoxCollider2D detectionWall;
    [SerializeField]
    private BoxCollider2D wallCollider;

    private void Awake()
    {
        if(detectionWall == null || wallCollider == null)
        {
            Debug.LogError("Set a detection and wall for the dash wall " + gameObject.name);
        }

        detectionWall.isTrigger = true;
        wallCollider.isTrigger = false;
    }

    protected virtual void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.TryGetComponent(out PlayerMagicCasting playerMagic))
        {
            if (playerMagic.IsDashingState)
            {
                wallCollider.enabled = false;
                detectionWall.enabled = false;
                Debug.Log("No more dash wall");
            }
        }
    }
}
