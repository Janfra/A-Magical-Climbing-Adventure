using System;
using UnityEngine;

public sealed class GameManager
{
    private static readonly GameManager instance = new();
    public InputManager inputManager;
    public SceneHandler sceneLoader;
    public static GameManager Instance
    {
        get { return instance; }
    }

    private GameManager()
    {
        inputManager = new();
        sceneLoader = new();
        Debug.Log("Instance created");
    }
}
