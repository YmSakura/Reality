using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    
    public AudioSource audioSource, bgm;
    [SerializeField]
    private AudioClip jumpAudio, hurtAudio, deathAudio;

    private void Awake()
    {
        instance = this;
    }

    public void JumpAudio()
    {
        audioSource.clip = jumpAudio;
        audioSource.Play();
    }

    public void HurtAudio()
    {
        audioSource.clip = hurtAudio;
        audioSource.Play();
    }

    public void DeathAudio()
    {
        audioSource.clip = deathAudio;
        audioSource.Play();
    }

}
