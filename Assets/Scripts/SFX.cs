using System.Collections;
using System.Collections.Generic;
using UnityEngine;
[RequireComponent(typeof(AudioSource))] // AudioSource 자동 추가

public class SFX : MonoBehaviour
{
    public AudioClip[] clips;
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void PlaySound(int kind)
    {
        audioSource.clip = clips[kind];
        audioSource.Play();
    }
}
