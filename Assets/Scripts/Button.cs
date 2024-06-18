using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Button : MonoBehaviour, IPointerDownHandler
{
    public Transform sfx;

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        sfx.SendMessage("AudioPlay", 4);
    }
}
