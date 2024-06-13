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
            audioSource.clip = clips[kind]; // ������ �´� ����� Ŭ�� ����
            audioSource.Play();
        }
    }

    void AudioStop(int kind)
    {
        if (audioSource.isPlaying)
        {
            audioSource.clip = clips[kind]; // ������ �´� ����� Ŭ�� ����
            audioSource.Stop();
        }
    }
}
