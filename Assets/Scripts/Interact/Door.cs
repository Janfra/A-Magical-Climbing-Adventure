using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour, IInteractable
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

        var inventory = interactor.GetComponent<Inventory>();
        if(inventory == null) return false;
        if(inventory.hasKey)
        {
            Debug.Log("opening door.");
            return true;
        }
        Debug.Log("no key found");
        return false;
        
    }
}