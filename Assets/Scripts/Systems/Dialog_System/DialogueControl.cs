using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;
public class DialogueControl : MonoBehaviour
{
    public static int currentIndex;       // ���� ����ϴ� ��ȭ�� �ε���
    public string scriptName;             // ��ȭâ �̸�
    public TMP_Text infoText;

    GameObject dialogueManager;

    void Awake()
    {
        dialogueManager = GameObject.Find("DialogueManager");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !GameManager.isMenu)
        {
            if (!Dialogue.instance.dialogueRunning && !Dialogue.instance.dialogue_read(currentIndex))
            {
                ScriptOn(scriptName);
            }

            if (Dialogue.instance.dialogue_cycles[currentIndex].check_cycle_read)
            {
                StartCoroutine(Info());
            }
        }
        if (Dialogue.instance.dialogue_cycles[5].check_cycle_read) // ���� Ŭ����
        {
            SceneManager.LoadScene("ClearScene");
        }
        Debug.Log(currentIndex);
    }

    // ��ũ��Ʈ ��� ����
    public void ScriptOn(string scriptName)
    {
        switch (scriptName)
        {
            case "Tutorial":
                currentIndex = 1;
                if (Dialogue.instance.dialogue_read(currentIndex) && !Dialogue.instance.dialogueRunning)
                {
                    IEnumerator dialogue_co = Dialogue.instance.dialogue_system_start(currentIndex);
                    StartCoroutine(dialogue_co);
                }
                break;
            case "Puzzle":
                currentIndex = 2;
                if (Dialogue.instance.dialogue_read(currentIndex) && !Dialogue.instance.dialogueRunning)
                {
                    IEnumerator dialogue_co = Dialogue.instance.dialogue_system_start(currentIndex);
                    StartCoroutine(dialogue_co);
                }
                break;
            case "SpeedRun":
                currentIndex = 3;
                if (Dialogue.instance.dialogue_read(currentIndex) && !Dialogue.instance.dialogueRunning)
                {
                    IEnumerator dialogue_co = Dialogue.instance.dialogue_system_start(currentIndex);
                    StartCoroutine(dialogue_co);
                }
                break;
            case "Defense":
                currentIndex = 4;
                if (Dialogue.instance.dialogue_read(currentIndex) && !Dialogue.instance.dialogueRunning)
                {
                    IEnumerator dialogue_co = Dialogue.instance.dialogue_system_start(currentIndex);
                    StartCoroutine(dialogue_co);
                }
                break;
            case "End":
                if (GameManager.isGameClear)
                currentIndex = 5;
                if (Dialogue.instance.dialogue_read(currentIndex) && !Dialogue.instance.dialogueRunning)
                {
                    IEnumerator dialogue_co = Dialogue.instance.dialogue_system_start(currentIndex);
                    StartCoroutine(dialogue_co);
                    dialogueManager.SendMessage("Reacts", "Black_In");
                }
                break;
        }
    }

    IEnumerator Info()
    {
        infoText.text = "<color=red>������ ��ȭ�� �� ����</color>";
        yield return new WaitForSeconds(1.0f);
        infoText.text = "";
    }

    public void OnDisable()
    {
        infoText.text = "";
    }
}
