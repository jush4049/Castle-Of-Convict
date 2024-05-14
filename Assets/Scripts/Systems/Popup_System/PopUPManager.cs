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

    // �ǽð� �˾� UI ���� LinkedList
    private LinkedList<PopUpHandler> activePopupLinkedList;

    // ��ü �˾� UI List
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

            // ESC Ű �Է� �� LinkedList�� ù ��° ���� ����
            if (Input.GetKeyDown(Popup_escapeKey))
            {
                if (activePopupLinkedList.Count > 0)
                {
                    ClosePopup(activePopupLinkedList.First.Value);
                }
            }
            // ����Ű ����
            ToggleKeyDownAction(Popup_TestKey0, popUpHandler[0]);
            ToggleKeyDownAction(Popup_TestKey1, popUpHandler[1]);
            ToggleKeyDownAction(Popup_TestKey2, popUpHandler[2]);
        }
    }

    private void Init()
    {
        // List �ʱ�ȭ
        // �˾� UI �߰��Ҷ����� ��� ����
        allPopupList = new List<PopUpHandler>()
        {
            popUpHandler[0], popUpHandler[1], popUpHandler[2]
            //menuPanel, guidePanel
        };

        // ��� �˾� UI�� �̺�Ʈ ���
        foreach (var popup in allPopupList)
        {
            // ��� ��Ŀ�� �̺�Ʈ
            popup.OnFocus += () =>
            {
                activePopupLinkedList.Remove(popup);
                activePopupLinkedList.AddFirst(popup);
                RefreshAllPopupDepth();
            };

            // �ݱ� ��ư �̺�Ʈ
            //popup._closeButton.onClick.AddListener(() => ClosePopup(popup));
        }
    }

    // ���� �� ��� �˾� UI�� ���� ����
    private void InitCloseAll()
    {
        foreach (var popup in allPopupList)
        {
            ClosePopup(popup);
        }
    }

    // ����Ű �Է¿� ���� �˾� UI�� ���� ����
    private void ToggleKeyDownAction(in KeyCode key, PopUpHandler popup)
    {
        if (Input.GetKeyDown(key))
            ToggleOpenClosePopup(popup);
    }

    // �˾� UI ���¿� ���� ���� ���� (Open/Close)
    public void ToggleOpenClosePopup(PopUpHandler popup)
    {
        if (!popup.gameObject.activeSelf) OpenPopup(popup);
        else ClosePopup(popup);
    }

    // �˾� UI�� ���� LinkedList ��ܿ� �߰�
    private void OpenPopup(PopUpHandler popup)
    {
        activePopupLinkedList.AddFirst(popup);
        popup.gameObject.SetActive(true);
        RefreshAllPopupDepth();
    }

    // �˾� UI�� �ݰ� LinkedList���� ����
    private void ClosePopup(PopUpHandler popup)
    {
        activePopupLinkedList.Remove(popup);
        popup.gameObject.SetActive(false);
        RefreshAllPopupDepth();
    }

    // LinkedList �� ��� �˾� UI �ڽ� ���� ���ġ
    private void RefreshAllPopupDepth()
    {
        foreach (var popup in activePopupLinkedList)
        {
            popup.transform.SetAsFirstSibling();
        }
    }
}
