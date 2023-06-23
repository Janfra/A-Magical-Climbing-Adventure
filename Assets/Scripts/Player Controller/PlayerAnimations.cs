using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class PlayerAnimations : MonoBehaviour
{
    [Header("Dependencies")]
    [SerializeField]
    SpriteRenderer spriteRenderer;
    [SerializeField]
    PlayerController controller;
    [SerializeField]
    PlayerMagicCasting magicCasting;
    [SerializeField]
    Animator playerAnimator;
    [SerializeField]
    ParticleSystem jumpParticles;
    [SerializeField]
    ParticleSystem landingParticles;

    private AudioManager audioPlayer;

    private void Awake()
    {
        if(playerAnimator == null)
        {
            playerAnimator = GetComponent<Animator>();
        }
    }

    private void Start()
    {
        controller.PlayerJump.OnGroundedOnce += PlayLandingParticles;
        controller.PlayerJump.OnJump += PlayJumpingParticles;
        // controller.MovementHandler.OnMove += PlaySlideSound;
        magicCasting.OnCastInProgress += SetCastingAnimation;
        audioPlayer = AudioManager.Instance;
    }

    private void OnDisable()
    {
        if(controller != null)
        {
            controller.PlayerJump.OnGroundedOnce -= PlayLandingParticles;
            controller.PlayerJump.OnJump -= PlayJumpingParticles;
        }

        if(magicCasting != null)
        {
            magicCasting.OnCastInProgress -= SetCastingAnimation;
        }
    }

    private void Update()
    {
        playerAnimator.SetBool("Dashing", magicCasting.IsDashingState);
        playerAnimator.SetFloat("FallSpeed", Mathf.Abs(controller.PlayerRigidbody.velocity.y));

        if (!controller.MovementHandler.isAffectedBySource)
        {
            playerAnimator.SetFloat("Moving", Mathf.Abs(controller.PlayerRigidbody.velocity.x));
        }
        else
        {
            playerAnimator.SetFloat("Moving", 0.0f);
        }
        
        if(controller.MovementHandler.MovementDirection.x > 0)
        {
            spriteRenderer.flipX = true;
        }
        else
        {
            spriteRenderer.flipX = false;
        }
    }

    private void PlaySlideSound(bool _isNotSliding)
    {
        if (!_isNotSliding)
        {
            if (!audioPlayer.IsAudioPlaying("Slide"))
            {
                audioPlayer.PlayAudio("Slide");
            }
        }
        else
        {
            audioPlayer.StopAudio("Slide");
        }
    }

    private void SetCastingAnimation(bool _isCasting)
    {
        playerAnimator.SetBool("Casting", _isCasting);
    }

    private void PlayLandingParticles()
    {
        audioPlayer.PlayAudio("Land");
        landingParticles.Play();
    }

    private void PlayJumpingParticles()
    {
        audioPlayer.PlayAudio("Jump");
        jumpParticles.Play();
    }
}
