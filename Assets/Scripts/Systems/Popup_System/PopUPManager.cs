using System.Collections.Generic;
using System.Data.SqlTypes;
using UnityEngine;

public class PopUPManager : MonoBehaviour
{
    public PopUpHandler[] popUpHandler;
    /*public PopUpHandler menuPanel;
    public PopUpHandler guidePanel;*/

    [Space]
    public KeyCode Popup_escapeKey = KeyCode.Escape;
    public KeyCode Popup_TestKey0 = KeyCode.F1;
    public KeyCode Popup_TestKey1 = KeyCode.G;
    public KeyCode Popup_TestKey2 = KeyCode.I;

    // 실시간 팝업 UI 관리 LinkedList
    private LinkedList<PopUpHandler> activePopupLinkedList;

    // 전체 팝업 UI List
    private List<PopUpHandler> allPopupList;

    private void Awake()
    {
        activePopupLinkedList = new LinkedList<PopUpHandler>();
        Init();
        //InitCloseAll();
        Cursor.visible = false;
    }

    private void Update()
    {
        if (!Dialogue.instance.dialogueRunning)
        {
            if (activePopupLinkedList.Count > 0)
            {
                GameManager.isMenu = true;
                Time.timeScale = 0;
                Cursor.visible = true;
            }
            else
            {
                GameManager.isMenu = false;
                Time.timeScale = 1;
            }

            // ESC 키 입력 시 LinkedList의 첫 번째 값을 닫음
            if (Input.GetKeyDown(Popup_escapeKey))
            {
                if (activePopupLinkedList.Count > 0)
                {
                    ClosePopup(activePopupLinkedList.First.Value);
                }
            }
            // 단축키 조작
            ToggleKeyDownAction(Popup_TestKey0, popUpHandler[0]);
            ToggleKeyDownAction(Popup_TestKey1, popUpHandler[1]);
            ToggleKeyDownAction(Popup_TestKey2, popUpHandler[2]);
        }
    }

    private void Init()
    {
        // List 초기화
        // 팝업 UI 추가할때마다 계속 갱신
        allPopupList = new List<PopUpHandler>()
        {
            popUpHandler[0], popUpHandler[1], popUpHandler[2]
            //menuPanel, guidePanel
        };

        // 모든 팝업 UI에 이벤트 등록
        foreach (var popup in allPopupList)
        {
            // 헤더 포커스 이벤트
            popup.OnFocus += () =>
            {
                activePopupLinkedList.Remove(popup);
                activePopupLinkedList.AddFirst(popup);
                RefreshAllPopupDepth();
            };

            // 닫기 버튼 이벤트
            //popup._closeButton.onClick.AddListener(() => ClosePopup(popup));
        }
    }

    // 시작 시 모든 팝업 UI는 닫은 상태
    private void InitCloseAll()
    {
        foreach (var popup in allPopupList)
        {
            ClosePopup(popup);
        }
    }

    // 단축키 입력에 따라 팝업 UI를 열고 닫음
    private void ToggleKeyDownAction(in KeyCode key, PopUpHandler popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }

    // 팝업 UI 상태에 따라 열고 닫음 (Open/Close)
    public void ToggleOpenClosePopup(PopUpHandler popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup);
    }

    // 팝업 UI를 열고 LinkedList 상단에 추가
    private void OpenPopup(PopUpHandler popup)
    {
        activePopupLinkedList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    // 팝업 UI를 닫고 LinkedList에서 제거
    private void ClosePopup(PopUpHandler popup)
    {
        activePopupLinkedList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    // LinkedList 내 모든 팝업 UI 자식 순서 재배치
    private void RefreshAllPopupDepth()
    {
        foreach (var popup in activePopupLinkedList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
