using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler
{
    AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        audioSource.Play();
    }
}
