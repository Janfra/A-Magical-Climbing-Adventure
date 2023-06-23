using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour, IInteractable
{
      public InteractionPromptUI interactionPromptUI { get; set; }

    [SerializeField] private string prompt;
    public string InteractionPrompt => prompt;

    private void Awake()
    {
        interactionPromptUI = GetComponentInChildren<InteractionPromptUI>();
    }

    public bool Interact(Interactor interactor)
    {
        Debug.Log("took item.");
        return true;
    }
}
