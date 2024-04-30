using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpHeaderHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField]
    private Transform popUpRect; // �̵��� UI
    [SerializeField]
    private Vector2 rectBegin;
    [SerializeField]
    private Vector2 moveBegin;
    [SerializeField]
    private Vector2 moveOffset;

    private void Awake()
    {
        // �̵� ��� UI�� �������� ���� ���, �θ�� �ʱ�ȭ
        if (popUpRect == null)
            popUpRect = transform.parent;
    }
    // ���콺 �巡�� ����
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        rectBegin = popUpRect.position; // �̵��� UI�� Vector2 ��ġ
        moveBegin = eventData.position; // �巡�׷� �����̴� ��ġ
    }

    void IDragHandler.OnDrag(PointerEventData eventData)
    {
        if (Input.GetMouseButton(0))
        {
            // transform.position = Input.mousePosition;
            moveOffset = eventData.position - moveBegin;
            popUpRect.position = rectBegin + moveOffset;
        }
    }
}
