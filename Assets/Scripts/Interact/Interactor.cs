using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Interactor : MonoBehaviour
{
    [SerializeField] private Transform interactionPoint;
    [SerializeField] private float interactionPointRadius = 1f;
    [SerializeField] private LayerMask interactableMask;

    private readonly Collider[] colliders = new Collider[3];
    private int numFound;
    private IInteractable interactable;

    private void Update() {

        //saves the objects it finds in it
        numFound = Physics.OverlapSphereNonAlloc(interactionPoint.position, interactionPointRadius, colliders, interactableMask);

        if(numFound > 0)
        {
            if(colliders[0].TryGetComponent(out IInteractable foundInteract))
            {
                interactable = foundInteract;
                interactable.interactionPromptUI.SetUp(interactable.InteractionPrompt);
                
                if(Keyboard.current != null)
                {
                    if (Keyboard.current.fKey.wasPressedThisFrame) interactable.Interact(this);
                }
            }
        }
        else
        {
            // Close UI on leave
            if(interactable != null)
            {
                Debug.Log("nicht leer");
                if(interactable.interactionPromptUI.IsDisplayed)
                {
                    Debug.Log("displayed");
                    interactable.interactionPromptUI.Close();
                }
            }  
            if(interactable != null) interactable = null;
        }
    }


    private void OnDrawGizmos() {
        if(interactionPoint != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(interactionPoint.position, interactionPointRadius);
        }
    }


}
