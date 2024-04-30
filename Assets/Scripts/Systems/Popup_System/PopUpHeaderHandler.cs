using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class PopUpHeaderHandler : MonoBehaviour, IBeginDragHandler, IDragHandler
{
    [SerializeField]
    private Transform popUpRect; // 이동할 UI
    [SerializeField]
    private Vector2 rectBegin;
    [SerializeField]
    private Vector2 moveBegin;
    [SerializeField]
    private Vector2 moveOffset;

    private void Awake()
    {
        // 이동 대상 UI를 지정하지 않은 경우, 부모로 초기화
        if (popUpRect == null)
            popUpRect = transform.parent;
    }
    // 마우스 드래그 동작
    void IBeginDragHandler.OnBeginDrag(PointerEventData eventData)
    {
        rectBegin = popUpRect.position; // 이동할 UI의 Vector2 위치
        moveBegin = eventData.position; // 드래그로 움직이는 위치
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
