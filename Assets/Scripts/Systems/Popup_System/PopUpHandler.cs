using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PopUpHandler : MonoBehaviour, IPointerDownHandler
{
    public event Action OnFocus;

    // �˾� UI�� ���콺�� Ŭ���� �� �̺�Ʈ�� �߻���Ŵ
    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        OnFocus();
        Debug.Log("�巡��");
    }
}
