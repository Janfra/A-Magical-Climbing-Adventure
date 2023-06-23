using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoadingCoroutineHandler : MonoBehaviour
{
    private static SceneLoadingCoroutineHandler instance;
    private static bool isInstanceCreated => instance != null;

    // Start is called before the first frame update
    void Start()
    {
        if(!isInstanceCreated)
        {
            instance = this;
            DontDestroyOnLoad(this);
            GameManager.Instance.sceneLoader.SetCoroutineHandler(instance);
        }
    }
}
