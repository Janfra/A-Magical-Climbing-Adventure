using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class LoadingBar
{
    private float targetValue, initialValue, currentValue, duration, time;

    public float CurrentValue => currentValue;
    public bool IsCompleted => targetValue == currentValue;

    private bool isTarget, isRemoving;

    private const float MIN_DURATION = 0.2f;
    private const float MAX_DURATION = 1f;

    public void OnStart()
    {
        duration = MAX_DURATION;
        targetValue = 1;
        currentValue = targetValue;
        initialValue = targetValue;
    }

    private void SetDuration(float _newDuration)
    {
        if(_newDuration <= MIN_DURATION)
        {
            duration = MIN_DURATION;
        }
        else
        {
            duration = _newDuration;
        }
    }

    public void SetLoadTarget<T>(float _targetValue, float _initialValue, T _caller) where T : MonoBehaviour
    {
        SetDuration(duration - 0.25f);
        targetValue = _targetValue;

        if (!isTarget)
        {
            isTarget = true;
            SetupLoading(_initialValue, MAX_DURATION);
            _caller.StartCoroutine(LoadToTarget());
        }
        else if(_targetValue < currentValue || _targetValue < initialValue)
        {
            SetupLoading(currentValue, MIN_DURATION);
            isRemoving = true;
        }
    }

    /// <summary>
    /// Sets 'initialValue' and restarts variables to then start loading towards target.
    /// </summary>
    /// <param name="_initialValue">New starting value</param>
    private void SetupLoading(float _initialValue, float _duration)
    {
        SetDuration(_duration);
        initialValue = _initialValue;
        isTarget = !IsCompleted;
        time = 0.0f;
    }

    /// <summary>
    /// Loads to the target while keeping the UI active until completation.
    /// </summary>
    IEnumerator LoadToTarget()
    {
        while (!IsCompleted && !isRemoving)
        {
            time += Time.deltaTime;
            currentValue = Mathf.Lerp(initialValue, targetValue, Mathf.Clamp01(time / duration));
            yield return null;
        }
        if (isRemoving)
        {
            while (!IsCompleted)
            {
                time += Time.deltaTime;
                currentValue = Mathf.Lerp(initialValue, targetValue, Mathf.Clamp01(time / duration));
                yield return null;
            }
        }

        isTarget = false;
        yield return null;
    }
}
