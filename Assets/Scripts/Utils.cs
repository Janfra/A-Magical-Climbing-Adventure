using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static float NormalizeFloats(float _currentValue, float _minValue, float _maxValue)
    {
        // 1 is added to everything to avoid dividing under 0 and getting unexpected values
        int decimalOffset = 1;
        _minValue += decimalOffset;
        _maxValue += decimalOffset;
        _currentValue += decimalOffset;
        return (_currentValue - _minValue) / (_maxValue - _minValue);
    }

    public static Color SetColourOpacity(Color _color, float _opacity)
    {
        Color color = _color;
        color.a = _opacity;
        return color;
    }

    public static IEnumerator OnChainedTimer(Timer _timer, Action _action, MonoBehaviour _caller)
    {
        yield return new WaitForEndOfFrame();
        _timer.StartTimer(_caller);
        _timer.SetTimerOnDoneAction(_action);
        yield return null;
    }

    public static IEnumerator OnLoopTimer(Timer _timer, MonoBehaviour _caller)
    {
        yield return new WaitForEndOfFrame();
        _timer.StartTimer(_caller);
        yield return null;
    }
}
