using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainCanvas : MonoBehaviour
{
    [SerializeField] PlayerMagicCasting playerMagicCasting;
    [SerializeField] LoadingBar manaLoading;

    private float maxHealth = 100;
    public float currentHealth;

    #region Lerping Variables

    public float timeToFill;
    private bool isHPLerping;
    private float healthLerpTime;
    private float targetHealthFill;

    #endregion

    [SerializeField] Image healthFillAmount;
    [SerializeField] Image manaFillAmount;


    [Header("Pause Menu")]
    [SerializeField] CanvasGroup pausedCanvas;
    [SerializeField] CanvasGroup tutorialCanvas;

    [SerializeField] private AudioClip hoverSound;
    [SerializeField] private AudioSource menuaudioListener;

    private void Awake() {
        ResumeGame();
    }

    private void Start() {
        SetBarValues();
        playerMagicCasting.OnManaUpdate += SetMana;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.Escape))
        {
            PauseGame();
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    private void SetBarValues(float healthFillValue = 1.0f, float manaFillValue = 1.0f)
    {
        healthFillAmount.fillAmount = healthFillValue;
        manaFillAmount.fillAmount = manaFillValue;
    }

    public void SetMana(float _targetValue, float _currentMana)
    {
        if (manaLoading.IsCompleted)
        {
            StartCoroutine(LerpFillMana(_targetValue, _currentMana));
        }
        else
        {
            manaLoading.SetLoadTarget(GetFillTarget(_targetValue, playerMagicCasting.MaxMana), GetFillTarget(_currentMana, playerMagicCasting.MaxMana), this);
        }
    }

    public void SetHealth(float _value)
    {
        if (!isHPLerping)
        {
            StartCoroutine(LerpFillHealth(_value));
        }
        else
        {

        }
    }

    public void ChangeBarValue(float health = 0, float mana = 0)
    {
        healthFillAmount.fillAmount += ((currentHealth + health)/100f);
        
        currentHealth = healthFillAmount.fillAmount;
    }

    IEnumerator LerpFillHealth(float targetValue, float duration = 1)
    {
        isHPLerping = true;
        healthLerpTime = 0.0f;
        float currentFill = Utils.NormalizeFloats(currentHealth, 0, maxHealth);
        targetHealthFill = Utils.NormalizeFloats(targetValue, 0, maxHealth);
        currentHealth = targetValue;
        yield return null;

        while (true)
        {
            healthFillAmount.fillAmount = Mathf.Lerp(currentFill, targetHealthFill, Mathf.Clamp01(healthLerpTime / duration));
            healthLerpTime += Time.deltaTime;
            if(healthFillAmount.fillAmount == targetHealthFill)
            {
                break;
            }
            yield return null;
        }

        isHPLerping = false;
        yield return null;
    }

    IEnumerator LerpFillMana(float _targetValue, float _currentMana, float duration = 1)
    {
        manaLoading.SetLoadTarget(GetFillTarget(_targetValue, playerMagicCasting.MaxMana), GetFillTarget(_currentMana, playerMagicCasting.MaxMana), this);
        yield return null;

        while (!manaLoading.IsCompleted)
        {
            manaFillAmount.fillAmount = manaLoading.CurrentValue;
            yield return null;
        }
        yield return null;
    }

    private float GetFillTarget(float _targetValue, float _maxValue)
    {
        return Utils.NormalizeFloats(_targetValue, 0, _maxValue);
    }

    //For Buttons and Pause Menu

    private void PauseGame()
    {
        //pause the game
        pausedCanvas.alpha = 1f;
        pausedCanvas.interactable = true;
        pausedCanvas.blocksRaycasts = true;
        Time.timeScale = 0f;
        GameManager.Instance.inputManager.PauseAllPlayerInputs();
    }
    public void ResumeGame()
    {
        pausedCanvas.alpha = 0f;
        pausedCanvas.interactable = false;
        pausedCanvas.blocksRaycasts = false;
        if(tutorialCanvas != null) tutorialCanvas.alpha = 0f;
        Cursor.lockState = CursorLockMode.Confined;
        Cursor.visible = true;
        Time.timeScale = 1;
        GameManager.Instance.inputManager.UnpauseAllPlayerInputs();
    }
    public void TutorialInfo()
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
    public void OptionsButton()
    {

    }

    public void QuitGame()
    {
        Time.timeScale = 1;
        GameManager.Instance.sceneLoader.LoadNextPlace("Menu");
    }
    public void PausedMouseHoverSound()
    {
        menuaudioListener.PlayOneShot(hoverSound);
    }
}


