using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogueControl : MonoBehaviour
{
    public static int currentIndex;       // ���� ����ϴ� ��ȭ�� �ε���
    public string scriptName;      // ��ȭâ �̸�
    public TMP_Text infoText;

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
        }
    }

    IEnumerator Info()
    {
        infoText.text = "<color=red>������ ��ȭ�Ҽ� ����</color>";
        yield return new WaitForSeconds(1.0f);
        infoText.text = "";
    }

    public void OnDisable()
    {
        infoText.text = "";
    }
}
