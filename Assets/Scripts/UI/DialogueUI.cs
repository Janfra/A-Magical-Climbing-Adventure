using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class DialogueUI : MonoBehaviour
{
    public event Action OnLoadComplete;
    public event Action OnHide;

    #region Variables & Constants

    [Header("Dependencies")]
    [SerializeField]
    RectTransform uiTransform;
    [SerializeField]
    TextMeshProUGUI dialogueText;
    [SerializeField]
    TextMeshProUGUI promptText;

    /// <summary>
    /// Main sprite is assumed to be always set, since it should be the player portrait
    /// </summary>
    [SerializeField]
    Image mainSprite;
    [SerializeField]
    Image secondarySprite;

    private bool isCancelled = false;
    private bool isLoading = false;
    public bool IsLoading => isLoading;
    private bool isVisible = false;
    public bool IsVisible => isVisible;
    private bool isDelayed = false;
    public string Text => dialogueText.text;
    private const float TEXT_LOADING_DELAY = 0.05f;

    private bool isOtherSet => secondarySprite.sprite != null;
    private const float NO_TALKING_OPACITY = 0.4f;
    private const float TALKING_OPACITY = 1.0f;

    #endregion

    /// <summary>
    /// Sets the static class to use this
    /// </summary>
    private void Awake()
    {
        SetImageOpacity(0.0f, mainSprite);
        SetImageOpacity(0.0f, secondarySprite);
        DialogueDisplay.SetDependencies(this);
    }

    /// <summary>
    /// Sets the text to load on screen
    /// </summary>
    /// <param name="_text"></param>
    public void SetText(string _text)
    {
        if (!isLoading)
        {
            CheckForOtherImage();
            SetImageOpacity(TALKING_OPACITY, mainSprite);
            StartCoroutine(LoadText(_text));
        }
        else
        {
            Debug.LogWarning("Tried to set dialogue text while is loading");
        }
    }

    /// <summary>
    /// Sets the text to load on screen and function to call when finished
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_onComplete"></param>
    public void SetText(string _text, Action _onComplete)
    {
        if (!isLoading)
        {
            CheckForOtherImage();
            SetImageOpacity(TALKING_OPACITY, mainSprite);
            OnLoadComplete += _onComplete;
            StartCoroutine(LoadText(_text));
        }
        else
        {
            Debug.LogWarning("Tried to set dialogue text while is loading");
        }
    }

    /// <summary>
    /// Sets the text to load on screen and function to call when finished
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_onComplete"></param>
    public void SetTextAndSelectImage(string _text, Action _onComplete, ImageSelected _image)
    {
        if (!isLoading)
        {
            SetImagesOpacities(_image);
            OnLoadComplete += _onComplete;
            StartCoroutine(LoadText(_text));
        }
        else
        {
            Debug.LogWarning("Tried to set dialogue text while is loading");
        }
    }

    /// <summary>
    /// Sets the sprites of the images being displayed
    /// </summary>
    /// <param name="_playerSprite"></param>
    /// <param name="_otherSprite"></param>
    public void SetSprites(Sprite _playerSprite, Sprite _otherSprite, bool _isOldKept = true)
    {
        if (_isOldKept)
        {
            if (_playerSprite)
            {
                mainSprite.sprite = _playerSprite;
            }
            if (secondarySprite)
            {
                secondarySprite.sprite = _otherSprite;
            }
        }
        else
        {
            mainSprite.sprite = _playerSprite;
            secondarySprite.sprite = _otherSprite;

            if (!_playerSprite)
            {
                SetImageOpacity(0.0f, mainSprite);
            }
            if (!secondarySprite)
            {
                SetImageOpacity(0.0f, secondarySprite);
            }
        }

    }

    /// <summary>
    /// Gives a function to call when text is loaded
    /// </summary>
    /// <param name="_onComplete"></param>
    public void AddOnTextCompleteDelegate(Action _onComplete)
    {
        OnLoadComplete += _onComplete;
    }

    /// <summary>
    /// Removes a function to call when text is loaded
    /// </summary>
    /// <param name="_onComplete"></param>
    public void RemoveOnTextCompleteDelegate(Action _onComplete)
    {
        OnLoadComplete -= _onComplete;
    }

    /// <summary>
    /// Gives a function to call when text is hidden
    /// </summary>
    /// <param name="_OnHide"></param>
    public void AddOnTextHideDelegate(Action _OnHide)
    {
        OnHide += _OnHide;
    }

    /// <summary>
    /// Removes a function to call when text is hidden
    /// </summary>
    /// <param name="_OnHide"></param>
    public void RemoveOnTextHideDelegate(Action _OnHide)
    {
        OnHide -= _OnHide;
    }

    /// <summary>
    /// Clears all events
    /// </summary>
    public void ClearEvents()
    {
        OnLoadComplete = null;
        OnHide = null;
    }

    /// <summary>
    /// Cancels text loading
    /// </summary>
    public void CancelLoading()
    {
        isCancelled = true;
    }

    /// <summary>
    /// Sets whether the UI is visible or not
    /// </summary>
    /// <param name="_isVisible"></param>
    public void AppearOnScreen(bool _isVisible)
    {
        // If already on that state dont do anything
        if(_isVisible == isVisible || IsLoading)
        {
            Debug.LogWarning("Tried to set dialogue visibility to the same or its loading.");
            return;
        }

        Vector2 newPosition;
        isVisible = _isVisible;

        if (_isVisible)
        {
            newPosition = new(uiTransform.anchoredPosition.x, 0);
            StartCoroutine(MoveUIPosition(newPosition, false));
        }
        else
        {
            newPosition = new(uiTransform.anchoredPosition.x, uiTransform.anchoredPosition.y - uiTransform.rect.height);
            StartCoroutine(MoveUIPosition(newPosition));
            OnHide?.Invoke();
            OnHide = null;
        }
    }

    /// <summary>
    /// Checks if other image is null, if false set it to no talking opacity
    /// </summary>
    private void CheckForOtherImage()
    {
        if (isOtherSet)
        {
            SetImageOpacity(NO_TALKING_OPACITY, secondarySprite);
        }
        else
        {
            SetImageOpacity(0.0f, secondarySprite);
        }
    }

    /// <summary>
    /// Sets opacity for both images
    /// </summary>
    /// <param name="_image"></param>
    private void SetImagesOpacities(ImageSelected _image)
    {
        bool isPlayer = false;
        Image selectedImage = GetImage(_image, out isPlayer);

        if (isPlayer)
        {
            SetImageOpacity(TALKING_OPACITY, selectedImage);
            CheckForOtherImage();
            return;
        }

        if (!isOtherSet && !isPlayer)
        {
            Debug.LogError("Other image not set!");
        }
        else if(!isPlayer)
        {
            SetImageOpacity(TALKING_OPACITY, selectedImage);
            SetImageOpacity(NO_TALKING_OPACITY, mainSprite);
        }

    }

    /// <summary>
    /// Sets the given image opacity to the given value
    /// </summary>
    /// <param name="_opacity"></param>
    /// <param name="_image"></param>
    private void SetImageOpacity(float _opacity, Image _image)
    {
        Color color = _image.color;
        color.a = _opacity;
        _image.color = color;
    }

    /// <summary>
    /// Gets the image being selected
    /// </summary>
    /// <param name="_image"></param>
    /// <param name="_isPlayer"></param>
    /// <returns></returns>
    private Image GetImage(ImageSelected _image, out bool _isPlayer)
    {
        switch (_image)
        {
            case ImageSelected.MainImage:
                _isPlayer = true;
                return mainSprite;
            case ImageSelected.SecondaryImage:
                _isPlayer = false;
                return secondarySprite;
            default:
                _isPlayer = false;
                return null;
        }
    }

    /// <summary>
    /// Moves the UI to the given position
    /// </summary>
    /// <param name="_position"></param>
    /// <returns></returns>
    private IEnumerator MoveUIPosition(Vector2 _position, bool _isSpriteHidden = true)
    {
        Vector2 currentPosition = uiTransform.anchoredPosition;
        float time = 0.0f;
        yield return null;

        if (_isSpriteHidden)
        {
            float mainImageOpacity = mainSprite.color.a;
            float secondaryImageOpacity = secondarySprite.color.a;

            while (true)
            {
                time += Time.deltaTime;
                time = Mathf.Clamp01(time);
                uiTransform.anchoredPosition = Vector2.Lerp(currentPosition, _position, time);
                SetImageOpacity(Mathf.Lerp(mainImageOpacity, 0, time), mainSprite);
                SetImageOpacity(Mathf.Lerp(secondaryImageOpacity, 0, time), secondarySprite);

                if (time == 1)
                {
                    break;
                }
                yield return null;
            }
        }
        else
        {
            while (true)
            {
                time += Time.deltaTime;
                uiTransform.anchoredPosition = Vector2.Lerp(currentPosition, _position, Mathf.Clamp01(time));

                if(uiTransform.anchoredPosition == _position)
                {
                    break;
                }
                yield return null;
            }
        }
    }

    /// <summary>
    /// Loads text character by character unless canceled
    /// </summary>
    /// <param name="_text"></param>
    /// <returns></returns>
    private IEnumerator LoadText(string _text)
    {
        isLoading = true;
        isCancelled = false;
        string tempString = "";
        dialogueText.text = "";
        promptText.text = "";
        yield return null;

        for (int i = 0; i < _text.Length && isLoading; i++)
        {
            if (isDelayed)
            {
                i--;
                yield return null;
                continue;
            }

            if (!isCancelled)
            {
                AudioManager.Instance.PlayAudio("Text_Boop");
                tempString += _text[i];
                yield return new WaitForSeconds(TEXT_LOADING_DELAY);
                dialogueText.text = tempString;
            }
            else
            {
                dialogueText.text = _text;
                yield return new WaitForSeconds(TEXT_LOADING_DELAY);
                break;
            }

        }

        promptText.text = "Press next";
        OnLoadComplete?.Invoke();
        OnLoadComplete = null;
        isLoading = false;
    }

    public enum ImageSelected
    {
        MainImage,
        SecondaryImage
    }
}

public static class DialogueDisplay
{
    /// <summary>
    /// Dialogue UI that will display the text
    /// </summary>
    private static DialogueUI dialogueOnScreen;

    // In case of timeout
    private static Timer timer;

    public static string DialogueText => dialogueOnScreen.Text;

    public static bool isLoading => dialogueOnScreen.IsLoading;

    /// <summary>
    /// Sets the UI to use
    /// </summary>
    /// <param name="_dialogueOnScreen"></param>
    public static void SetDependencies(DialogueUI _dialogueOnScreen)
    {
        if (dialogueOnScreen)
        {
            dialogueOnScreen.ClearEvents();
            dialogueOnScreen.CancelLoading();
            dialogueOnScreen.AppearOnScreen(false);
            Debug.LogWarning("Old dialogue replaced, events cleared");
        }

        dialogueOnScreen = _dialogueOnScreen;
        timer = new();
    }

    /// <summary>
    /// Sets the dialogue text on screen
    /// </summary>
    /// <param name="_text"></param>
    public static void SetDialogueText(string _text)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        if (dialogueOnScreen.IsVisible)
        {
            dialogueOnScreen.SetText(_text);
        }
        else
        {
            dialogueOnScreen.AppearOnScreen(true);
            dialogueOnScreen.SetText(_text);
        }
    }

    /// <summary>
    /// Sets the dialogue text on screen and function to call once completed
    /// </summary>
    /// <param name="_text"></param>
    /// <param name="_onComplete"></param>
    public static void SetDialogueText(string _text, Action _onComplete)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        if (dialogueOnScreen.IsVisible)
        {
            dialogueOnScreen.SetText(_text, _onComplete);
        }
        else
        {
            dialogueOnScreen.AppearOnScreen(true);
            dialogueOnScreen.SetText(_text, _onComplete);
        }
    }

    public static void SetDialogueTextAndSelectImage(string _text, Action _onComplete, DialogueUI.ImageSelected _imageSelected)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.SetTextAndSelectImage(_text, _onComplete, _imageSelected);
    }

    public static void SetDialogueSprites(Sprite mainSprite, Sprite secondarySprite, bool _isOldKept = true)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.SetSprites(mainSprite, secondarySprite, _isOldKept);
    }

    public static void CancelLoading()
    {
        dialogueOnScreen.CancelLoading();
    }

    /// <summary>
    /// Sets whether the UI is on screen
    /// </summary>
    /// <param name="_isVisible"></param>
    public static void SetVisibility(bool _isVisible)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.AppearOnScreen(_isVisible);
    }

    /// <summary>
    /// Adds given function to call when UI hides
    /// </summary>
    /// <param name="_onHide"></param>
    public static void SubscribeToHideEvent(Action _onHide)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.OnHide += _onHide;
    }

    /// <summary>
    /// Adds given function to call when UI text has loaded
    /// </summary>
    /// <param name="_onLoad"></param>
    public static void SubscribeToLoadEvent(Action _onLoad)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.OnLoadComplete += _onLoad;
    }

    /// <summary>
    /// Removes given function to call when UI hides
    /// </summary>
    /// <param name="_onHide"></param>
    public static void UnsubscribeToHideEvent(Action _onHide)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.OnHide -= _onHide;
    }

    /// <summary>
    /// Removes given function to call when UI text has loaded
    /// </summary>
    /// <param name="_onLoad"></param>
    public static void UnsubscribeToLoadEvent(Action _onLoad)
    {
        if (AreDependenciesMissing())
        {
            return;
        }

        dialogueOnScreen.OnLoadComplete -= _onLoad;
    }

    /// <summary>
    /// Checks that the dependencies required to function are in place
    /// </summary>
    /// <returns></returns>
    private static bool AreDependenciesMissing()
    {
        if(dialogueOnScreen == null)
        {
            Debug.LogError("No dialogue set to display on Dialogue Display, it needs an active Dialogue UI on scene");
            return true;
        }

        return false;
    }
}