using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopUpHandler : MonoBehaviour, IPointerDownHandler
{
    public event Action OnFocus;

    // 팝업 UI를 마우스로 클릭할 때 이벤트를 발생시킴
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        OnFocus();
        Debug.Log("드래그");
    }
}
