using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IInteractable
{
    public InteractionPromptUI interactionPromptUI { get; set; }
    public string InteractionPrompt {get;}
    public bool Interact(Interactor interactor );
}
