using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public abstract class DialoguePlayer : MonoBehaviour, IInputListener
{
    public event Action OnDialogueComplete;

    public bool isDisableAfterComplete = true;

    [System.Serializable]
    protected struct DialogueInformation
    {
        public bool isImageChanged => mainImage != null || secondaryImage != null;
        public string text;
        public DialogueUI.ImageSelected talker;
        public Sprite mainImage;
        public Sprite secondaryImage;
        public bool keepOldImages;
    }

    [SerializeField]
    protected List<DialogueInformation> dialogues;
    protected bool isInputGiven;

    private void Start()
    {
        IInputListener.AttemptToSubscribe(this, false);
    }

    private void OnDisable()
    {
        OnDialogueComplete = null;
        StopAllCoroutines();
    }

    private void LoadNextDialogue(InputAction.CallbackContext obj)
    {
        isInputGiven = true;
    }

    protected void StartDisplayDialogues()
    {
        if (dialogues.Count < 1)
        {
            Debug.LogError($"No dialogues set on {gameObject.name}");
            return;
        }

        StartCoroutine(DisplayDialogues());
    }

    protected virtual IEnumerator DisplayDialogues()
    {
        DialogueDisplay.SubscribeToHideEvent(EnableCharacterMovement);
        GameManager.Instance.inputManager.PauseAllPlayerInputs();
        int indexCount = 1;

        // Display first dialogue
        DialogueDisplay.SetVisibility(true);
        DialogueInformation dialogue;
        dialogue = dialogues[0];
        DisplayDialogueInformation(dialogue);
        yield return null;

        while(indexCount <= dialogues.Count)
        {
            if (isInputGiven && !DialogueDisplay.isLoading)
            {
                if(indexCount < dialogues.Count)
                {
                    dialogue = dialogues[indexCount];
                    DisplayDialogueInformation(dialogue);
                }

                indexCount++;
            }
            else if(isInputGiven)
            {
                DialogueDisplay.CancelLoading();
            }

            isInputGiven = false;
            yield return null;
        }
        DialogueDisplay.SetVisibility(false);
        OnDialogueComplete?.Invoke();
        gameObject.SetActive(isDisableAfterComplete);
        yield return null;
    }

    private void DisplayDialogueInformation(DialogueInformation _dialogueInformation)
    {
        if (_dialogueInformation.isImageChanged)
        {
            DialogueDisplay.SetDialogueSprites(_dialogueInformation.mainImage, _dialogueInformation.secondaryImage, _dialogueInformation.keepOldImages);
            Debug.LogWarning("No images were changed in the dialogue, keeping most recent ones");
        }
        DialogueDisplay.SetDialogueTextAndSelectImage(_dialogueInformation.text, null, _dialogueInformation.talker);
    }

    private void EnableCharacterMovement()
    {
        GameManager.Instance.inputManager.UnpauseAllPlayerInputs();
    }

    public void SubscribeToEvent(AMagicalClimbingAdventure _inputs)
    {
        _inputs.UI.AnyAction.started += LoadNextDialogue;
    }

    public void UnsubscribeToEvent(AMagicalClimbingAdventure _inputs, bool _isTemporary = true)
    {
        _inputs.UI.AnyAction.started -= LoadNextDialogue;
    }
}
