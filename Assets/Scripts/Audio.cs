using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Audio : MonoBehaviour
{
    public AudioClip[] clips;

    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void AudioPlay(int kind)
    {
        if (!audioSource.isPlaying)
        {
            audioSource.clip = clips[kind]; // 종류에 맞는 오디오 클립 설정
            audioSource.Play();
        }
    }

    void AudioStop(int kind)
    {
        if (audioSource.isPlaying)
        {
            audioSource.clip = clips[kind]; // 종류에 맞는 오디오 클립 설정
            audioSource.Stop();
        }
    }
}
