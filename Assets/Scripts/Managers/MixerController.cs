using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class MixerController : MonoBehaviour
{
    [SerializeField] AudioMixer myAudioMixer;
    [SerializeField] AudioClip audioClip;
    [SerializeField] AudioSource audioSource;

    public void SetVolume(float sliderValue)
    {
        myAudioMixer.SetFloat("MasterVolume", Mathf.Log10(sliderValue) * 20 );
        audioSource.PlayOneShot(audioClip, 0.2f);
        
    }
}