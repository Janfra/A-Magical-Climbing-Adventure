using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(DialoguePlayer))]
public class OnDialogueStateSwap : ChangeActiveStateOnEvent
{
    [SerializeField]
    DialoguePlayer dialogueEventHolder;

    private void Awake()
    {
        if(dialogueEventHolder == null)
        {
            Debug.LogWarning("No dialogue player set on the active state change event for " + gameObject.name);
            dialogueEventHolder = GetComponent<DialoguePlayer>();
        }

        dialogueEventHolder.OnDialogueComplete += OnEventCalled;
    }

    protected override void OnDisable()
    {
        dialogueEventHolder.OnDialogueComplete -= OnEventCalled;
    }
}
