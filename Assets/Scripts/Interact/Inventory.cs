using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Inventory : MonoBehaviour
{
    public bool hasKey = false;

    private void Update() 
    {
        if (Keyboard.current != null)
        {
            if (Keyboard.current.qKey.wasPressedThisFrame) hasKey = !hasKey;
        }
    }
}
