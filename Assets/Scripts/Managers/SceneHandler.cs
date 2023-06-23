using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneHandler
{
    public event Action OnSceneChange;
    private SceneLoadingCoroutineHandler coroutineHandler;

    public SceneHandler()
    {
        SceneManager.activeSceneChanged += SceneUpdate;
    }

    public void SetCoroutineHandler(SceneLoadingCoroutineHandler _sceneCoroutineHandler)
    {
        coroutineHandler = _sceneCoroutineHandler;
    }

    private void SceneUpdate(Scene _oldScene, Scene _newScene)
    {
        if(_newScene != null)
        {
            Debug.Log("New scene: " + _newScene.name);
            AudioManager.Instance.PlayMusicByIndex(_newScene.buildIndex);
        }
    }

    public void LoadNextPlace(string levelName)
    {
        if(coroutineHandler == null)
        {
            Debug.LogError("Add a Scene Coroutine Handler to the scene to load scenes");
            return;
        }

        if(SceneManager.GetSceneByName(levelName) == null)
        {
            Debug.Log($"Scene {levelName} not found");
            return;
        }

        if(!SceneManager.GetSceneByName(levelName).isLoaded)
        {
            coroutineHandler.StartCoroutine(LoadAsyncScene(levelName));
        }
    }

    public void LoadNextPlace(int _sceneIndex)
    {
        if (coroutineHandler == null)
        {
            Debug.LogError("Add a Scene Coroutine Handler to the scene to load scenes");
            return;
        }

        if (SceneManager.GetSceneByBuildIndex(_sceneIndex) == null)
        {
            Debug.Log($"Scene {_sceneIndex} not found");
            return;
        }

        if (!SceneManager.GetSceneByBuildIndex(_sceneIndex).isLoaded)
        {
            coroutineHandler.StartCoroutine(LoadAsyncScene(_sceneIndex));
        }
    }

    /// <summary>
    /// Start loading scene, and clear all input events before objects are destroyed
    /// </summary>
    /// <param name="_levelName"></param>
    /// <returns></returns>
    private IEnumerator LoadAsyncScene(string _levelName)
    {
        GameManager.Instance.inputManager.ClearAllEvents();
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_levelName);
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading scene, current progress: {asyncLoad.progress}");
            yield return null;
        }
    }

    /// <summary>
    /// Start loading scene, and clear all input events before objects are destroyed
    /// </summary>
    /// <param name="_levelName"></param>
    /// <returns></returns>
    private IEnumerator LoadAsyncScene(int _sceneIndex)
    {
        GameManager.Instance.inputManager.ClearAllEvents();
        yield return null;
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(_sceneIndex);
        while (!asyncLoad.isDone)
        {
            Debug.Log($"Loading scene, current progress: {asyncLoad.progress}");
            yield return null;
        }
    }
}