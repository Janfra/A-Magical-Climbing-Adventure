using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Users;

public class InputManager
{
    public event Action<bool> OnDeviceUpdate;

    private AMagicalClimbingAdventure inputs;
    public bool IsGamepad { get; private set; }
    private List<IInputListener> playerInputListeners;
    private List<IInputListener> UIInputListeners;

    public InputManager()
    {
        inputs = new();
        inputs.Enable();
        playerInputListeners = new();
        UIInputListeners = new();

        InputSystem.onDeviceChange += OnDeviceChanged;
        InputUser.onChange += OnControlSchemeChanged;
    }

    public void OnDestroy()
    {
        ClearAllEvents();
        inputs.Disable();
        inputs.Dispose();
    }

    #region Input Events Handling

    public void SubscribeToPlayerInput(IInputListener _listenerToSubscribe)
    {
        if (playerInputListeners.Contains(_listenerToSubscribe))
        {
            Debug.LogWarning("Already subscribed");
            return;
        }
        else
        {
            playerInputListeners.Add(_listenerToSubscribe);
            _listenerToSubscribe.SubscribeToEvent(inputs);
        }
    }

    public void UnsubscribeToPlayerInput(IInputListener _listenerToUnsubscribe)
    {
        if (playerInputListeners.Contains(_listenerToUnsubscribe))
        {
            _listenerToUnsubscribe.UnsubscribeToEvent(inputs);
            playerInputListeners.Remove(_listenerToUnsubscribe);
        }
        else
        {
            Debug.LogWarning("Not subscribed!");
        }
    }

    public void SubscribeToUIInput(IInputListener _listenerToSubscribe)
    {
        if (UIInputListeners.Contains(_listenerToSubscribe))
        {
            Debug.LogWarning("Already subscribed");
            return;
        }
        else
        {
            UIInputListeners.Add(_listenerToSubscribe);
            _listenerToSubscribe.SubscribeToEvent(inputs);
        }
    }

    public void UnsubscribeToUIInput(IInputListener _listenerToUnsubscribe)
    {
        if (UIInputListeners.Contains(_listenerToUnsubscribe))
        {
            _listenerToUnsubscribe.UnsubscribeToEvent(inputs);
            UIInputListeners.Remove(_listenerToUnsubscribe);
        }
        else
        {
            Debug.LogWarning("Not subscribed!");
        }
    }

    public void PauseAllPlayerInputs()
    {
        foreach (IInputListener listener in playerInputListeners)
        {
            listener.UnsubscribeToEvent(inputs);
        }
    }

    public void UnpauseAllPlayerInputs()
    {
        foreach (IInputListener listener in playerInputListeners)
        {
            listener.SubscribeToEvent(inputs);
        }
    }

    public void ClearPlayerEvents()
    {
        if (playerInputListeners.Count > 0)
        {
            foreach (IInputListener listener in playerInputListeners)
            {
                listener.UnsubscribeToEvent(inputs, false);
            }
            playerInputListeners.Clear();
        }
    }

    public void ClearAllEvents()
    {
        ClearPlayerEvents();
        if(UIInputListeners.Count > 0)
        {
            foreach (IInputListener listener in UIInputListeners)
            {
                listener.UnsubscribeToEvent(inputs, false);
            }
            UIInputListeners.Clear();
        }
    }

    #endregion

    #region Control Changing Handling

    /// <summary>
    /// Updates based on device change
    /// </summary>
    /// <param name="_device"></param>
    /// <param name="_deviceChange"></param>
    private void OnDeviceChanged(InputDevice _device, InputDeviceChange _deviceChange)
    {
        // Log info
        string deviceName = _device.ToString();
        // Debug.Log($"Device {_deviceChange}: {deviceName}, Device type: {_device.displayName}");

        // Is gamepad available now
        bool isGamepadAvailable = Gamepad.current != null;

        switch (_deviceChange)
        {
            case InputDeviceChange.Added:
            case InputDeviceChange.Reconnected:
                if (isGamepadAvailable && IsConnectedDeviceGamepad(_device))
                {
                    IsGamepad = isGamepadAvailable;
                    OnDeviceUpdate?.Invoke(isGamepadAvailable);
                    Debug.Log("Gamepad available, setting controllers to gamepad!");
                }
                break;

            case InputDeviceChange.Removed:
            case InputDeviceChange.Disconnected:
                IsGamepad = isGamepadAvailable;
                OnDeviceUpdate?.Invoke(isGamepadAvailable);
                if (!isGamepadAvailable)
                {
                    Debug.Log("No gamepad controllers available, switching to keyboard controllers!");
                }
                break;

            default:
                break;
        }
    }

    /// <summary>
    /// Check if device given is in the gamepad list
    /// </summary>
    /// <param name="_device">Deviced being checked</param>
    /// <returns></returns>
    private bool IsConnectedDeviceGamepad(InputDevice _device)
    {
        bool isConnectedDeviceGamepad = false;
        Gamepad isGamepad = (Gamepad)_device;

        if (isGamepad != null)
        {
            foreach (Gamepad gamepad in Gamepad.all)
            {
                if (gamepad == isGamepad)
                {
                    isConnectedDeviceGamepad = true;
                    break;
                }
            }
        }
        else
        {
            Debug.Log("Connected device is not a gamepad");
        }

        return isConnectedDeviceGamepad;
    }

    /// <summary>
    /// Calls event to update current input device type used
    /// </summary>
    /// <param name="_inputUser">Current control scheme used</param>
    /// <param name="_inputChange">Type of input change</param>
    private void OnControlSchemeChanged(InputUser _inputUser, InputUserChange _inputChange, InputDevice _device)
    {
        switch (_inputChange)
        {
            case InputUserChange.ControlSchemeChanged:
                // Get scheme name and check if it is gamepad
                string currentSchemeName = _inputUser.controlScheme.Value.name;
                IsGamepad = currentSchemeName == "Gamepad";

                // Update controller type
                OnDeviceUpdate?.Invoke(IsGamepad);
                Debug.Log($"Current Scheme is: {currentSchemeName}");
                break;

            default:
                break;
        }
    }

    #endregion
}

public interface IInputListener
{
    /// <summary>
    /// Attemps to subscribe listener to events checking for null
    /// </summary>
    /// <param name="_listener"></param>
    /// <returns>true if listener was successfully subscribed</returns>
    public static bool AttemptToSubscribe(IInputListener _listener, bool _isPlayerInput = true)
    {
        if (GameManager.Instance != null)
        {
            if (_isPlayerInput)
            {
                GameManager.Instance.inputManager.SubscribeToPlayerInput(_listener);
            }
            else
            {
                GameManager.Instance.inputManager.SubscribeToUIInput(_listener);
            }
            return true;
        }
        return false;
    }

    /// <summary>
    /// Function to add listener back to event
    /// </summary>
    public void SubscribeToEvent(AMagicalClimbingAdventure _inputs);

    /// <summary>
    /// Function to remove listener from event
    /// </summary>
    public void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true);
}