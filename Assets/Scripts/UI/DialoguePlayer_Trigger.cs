using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialoguePlayer_Trigger : DialoguePlayer
{
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.TryGetComponent(out PlayerController playerController))
        {
            playerController.PlayerRigidbody.velocity = new();
            StartDisplayDialogues();
        }
    }
}
