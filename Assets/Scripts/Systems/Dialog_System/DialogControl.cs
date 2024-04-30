using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DialogControl : MonoBehaviour
{
    public static int currentIndex;       // 현재 출력하는 대화의 인덱스
    public string scriptName;      // 대화창 이름
    public TMP_Text infoText;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.F) && !GameManager.isMenu)
        {
            if (!Dialog.instance.dialogRunning && !Dialog.instance.dialog_read(currentIndex))
            {
                ScriptOn(scriptName);
            }

            if (Dialog.instance.dialog_cycles[currentIndex].check_cycle_read)
            {
                StartCoroutine(Info());
            }
        }
    }

    // 스크립트 출력 시작
    public void ScriptOn(string scriptName)
    {
        switch (scriptName)
        {
            case "Tutorial":
                currentIndex = 1;
                if (Dialog.instance.dialog_read(currentIndex) && !Dialog.instance.dialogRunning)
                {
                    IEnumerator dialog_co = Dialog.instance.dialog_system_start(currentIndex);
                    StartCoroutine(dialog_co);
                }
                break;
            case "Puzzle":
                currentIndex = 2;
                if (Dialog.instance.dialog_read(currentIndex) && !Dialog.instance.dialogRunning)
                {
                    IEnumerator dialog_co = Dialog.instance.dialog_system_start(currentIndex);
                    StartCoroutine(dialog_co);
                }
                break;
        }
    }

    IEnumerator Info()
    {
        infoText.text = "<color=red>지금은 대화할수 없다</color>";
        yield return new WaitForSeconds(1.0f);
        infoText.text = "";
    }

    public void OnDisable()
    {
        infoText.text = "";
    }
}
