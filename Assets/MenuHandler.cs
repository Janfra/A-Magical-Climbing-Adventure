using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuHandler : MonoBehaviour
{
    [SerializeField] CanvasGroup tutorialCanvas;
    // Start is called before the first frame update
    void Start()
    {
       if(tutorialCanvas.isActiveAndEnabled) OptionsButton();
    }

    public void StartGame()
    {
        GameManager.Instance.sceneLoader.LoadNextPlace(SceneManager.GetActiveScene().buildIndex + 1);
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 1;
    }
  
    public void OptionsButton()
    {
        if(tutorialCanvas.alpha < 0.5f)
        {
            tutorialCanvas.alpha = 1f;
            tutorialCanvas.blocksRaycasts = true;
            tutorialCanvas.interactable = true;
        }
        else
        {
            tutorialCanvas.alpha = 0f;
            tutorialCanvas.blocksRaycasts = false;
            tutorialCanvas.interactable = false;
        }
    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        Application.Quit();
    }

    public void PausedMouseHoverSound()
    {
        AudioManager.Instance.PlayAudio("ClickSound7");
    }
}
