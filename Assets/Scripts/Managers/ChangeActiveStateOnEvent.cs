using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ChangeActiveStateOnEvent : MonoBehaviour
{
    [SerializeField]
    List<GameObject> objectToSwapState;

    protected abstract void OnDisable();

    protected void OnEventCalled()
    {
        foreach (GameObject gameObject in objectToSwapState)
        {
            gameObject.SetActive(!gameObject.activeSelf);
        }
    }
}
