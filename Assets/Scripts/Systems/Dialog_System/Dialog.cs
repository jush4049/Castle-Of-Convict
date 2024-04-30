using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class Dialog_info
{
    public string name;       // ��ȭ�� ȭ��
    [TextArea(3, 5)]
    public string content;    // ��ȭ ����
    public bool check_read;   // ���� ��ȭ�� �о����� Ȯ��
    public bool check_answer; // �亯�� ��������
    public string answer;     // �亯 ����
}

[System.Serializable]
public class Dialog_cycle
{
    public string cycle_name;
    public List<Dialog_info> info = new List<Dialog_info>();
    public int cycle_index;
    public bool check_cycle_read;
}


public class Dialog : MonoBehaviour
{
    [SerializeField]
    public static Dialog instance = null;
    public List<Dialog_cycle> dialog_cycles = new List<Dialog_cycle>(); // ��ȭ ���� �׷�
    public Queue<string> text_seq = new Queue<string>();                // ��ȭ �������� ������ ť�� �����Ѵ�.(������ ���� �Ǵ��ϱ� ����)
    public string name_;                                                // �ӽ÷� ������ ��ȭ ������ ����
    public string text_;                                                

    public TMP_Text nameText;                                            // ��ȭ�� ȭ��
    public TMP_Text dialogText;                                          // ��ȭ ���� ����
    public TMP_Text answerText;                                          // ��ȭ�� ���� �亯 
    public GameObject dialogCanvas;                                      // ��ȭ ���� ������Ʈ
    public GameObject answerButton;                                      // ���� ��ȭ ��¿� ��ư
    IEnumerator seq_;
    IEnumerator skip_seq;

    private float delay = 0.1f;                                          // ��ȭ ��� ������
    public bool dialogRunning = false;                                   // ��ȭ ��� ����

    public GameObject mainUI;
    public GameObject nextImage;

    public bool isAnswer = false;

    public int readIndex;

    void Awake()
    {
        answerButton.SetActive(false);
        dialogCanvas.SetActive(false);
        if (instance == null)
            instance = this;

        else if (instance != this)
            Destroy(gameObject);

        // DontDestroyOnLoad(gameObject);

        mainUI.SetActive(false);
        nextImage.SetActive(false);

        readIndex = 1;
    }
    
    void Start()
    {
        IEnumerator dialog_co = dialog_system_start(0);
        StartCoroutine(dialog_co);
    }

    public void skip(int index)
    {
        index = DialogControl.currentIndex;
        Debug.Log("���� ���̾�α�index : " + index);
        Debug.Log("readIndex : " + readIndex);
        Debug.Log(dialog_cycles[index].info.Count);
        StopAllCoroutines();                                // ��� �ڷ�ƾ ��Ȱ��ȭ
        dialogCanvas.SetActive(false);                      // ���̾�α� UI ��Ȱ��ȭ
        dialog_cycles[index].check_cycle_read = true;       // ���� ���̾�α� ���� ó��
        dialogRunning = false;                              // ���� ���� �ƴ�
        mainUI.SetActive(true);

        //dialog_cycles[index].info[readIndex - 1].check_read = true;
        for (int i = readIndex; i < dialog_cycles[index].info.Count; i++)       // ��ȭ ������ ������� ���
        {
            nameText.text = dialog_cycles[index].info[i].name;
            text_ = text_seq.Dequeue();                                  // ��ȭ ������ pop

            dialog_cycles[index].info[i].check_read = true;
        }
        readIndex = 1;
    }

    public IEnumerator dialog_system_start(int index)               // ���̾�α� ��� ����
    {
        answerButton.SetActive(false);
        nameText = dialogCanvas.GetComponent<parameter>().characterName;   // ������Ʈ���� �� ���� �޾ƿ���
        dialogText = dialogCanvas.GetComponent<parameter>().dialog;
        answerText = dialogCanvas.GetComponent<parameter>().answer;

        Cursor.visible = true;
        dialogRunning = true;
        mainUI.SetActive(false);
        foreach (Dialog_info dialog_temp in dialog_cycles[index].info)  // ��ȭ ������ ť�� �����ϱ� ���� �ִ´�.
        {
            Debug.Log("ǥ���� ���̾�α� ����" + index);
            Debug.Log(dialog_temp.content);
            text_seq.Enqueue(dialog_temp.content);                      // Enqueue > ť�� ������ �߰�
        }

        dialogCanvas.gameObject.SetActive(true);

        for (int i = 0; i < dialog_cycles[index].info.Count; i++)       // ��ȭ ������ ������� ���
        {
            nameText.text = dialog_cycles[index].info[i].name;
            // text_ = dialog_cycles[index].info[i].content;
            text_ = text_seq.Dequeue();                                  // ��ȭ ������ pop

            seq_ = seq_sentence(index, i);                               // ��ȭ ���� ��� �ڷ�ƾ
            StartCoroutine(seq_);                                        // �ڷ�ƾ ����


            yield return new WaitUntil(() =>                            // �۾��� �Ϸ�� ������ ��ٸ�
            {
                if (dialog_cycles[index].info[i].check_read)            // ���� ��ȭ�� �о����� �ƴ���
                {
                    return true;                                        // �о��ٸ� ����
                }
                else
                {
                    return false;                                       // ���� �ʾҴٸ� �ٽ� �˻�
                }
            });
        }
        dialog_cycles[index].check_cycle_read = true;                   // �ش� ��ȭ �׷� ����
        dialogRunning = false;
    }

    public void DisplayNext(int index, int number)                      // ���� �������� �Ѿ��
    {
        readIndex++;
        if (text_seq.Count == 0)                                        // ���� ������ ���ٸ�
        {
            dialogCanvas.gameObject.SetActive(false);                   // ���̾�α� ����
            mainUI.SetActive(true);                                     // ���� UI Ȱ��ȭ
            readIndex = 1;
        }
        StopCoroutine(seq_);                                            // �������� �ڷ�ƾ ����
        dialog_cycles[index].info[number].check_read = true;            // ���� ���� �������� ǥ��
    }

    public IEnumerator seq_sentence(int index, int number)              // ���� �ؽ�Ʈ �ѱ��� �� ���� ���
    {
        skip_seq = touch_wait(seq_, index, number);                     // ��ġ ��ŵ�� ���� ��ġ ��� �ڷ�ƾ �Ҵ�
        StartCoroutine(skip_seq);                                       // ��ġ ��� �ڷ�ƾ ����
        dialogText.text = "";                                           // ��ȭ ���� �ʱ�ȭ
        foreach (char letter in text_.ToCharArray())                    // ��ȭ ���� �ѱ��� �� �̾Ƴ���
        {
            dialogText.text += letter;                                  // �ѱ��ھ� ���
            yield return new WaitForSeconds(delay);                     // ��� ������
        }
        StopCoroutine(skip_seq);                                        // ���� ����� ������ ��ġ ��� �ڷ�ƾ ����
        nextImage.SetActive(true);

        if (dialog_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialog_cycles[index].info[number].answer;
            IEnumerator next = next_touch(index, number);                   // ��ư �̿ܿ� �κ��� ��ġ�ص� �Ѿ�� �ڷ�ƾ ����
            StartCoroutine(next);
        }
        else
        {
            IEnumerator next = next_touch(index, number);
            StartCoroutine(next);
        }
    }

    public IEnumerator touch_wait(IEnumerator seq, int index, int number)                                // ��ġ ��� �ڷ�ƾ
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space));  // ��Ŭ�� �Ǵ� �����̽� �� ��ư���� ����
        StopCoroutine(seq);                                                                              // ��ȭ ���� �ڷ�ƾ ����
        dialogText.text = text_;                                                                         // ��� ���� �ѹ��� ���
        nextImage.SetActive(true);
        if (dialog_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialog_cycles[index].info[number].answer;
        }
        IEnumerator next = next_touch(index, number);                                                 // ��ȭ ���� �ڷ�ƾ�� ���� �Ǿ� ���� �������� ���� �ڷ�ƾ�� ����
        StartCoroutine(next);
    }

    public IEnumerator next_touch(int index, int number)                                              // ���� �������� �Ѿ�� �ڷ�ƾ
    {
        StopCoroutine(seq_);
        StopCoroutine(skip_seq);
        yield return new WaitForSeconds(0.3f);
        if (dialog_cycles[index].info[number].check_answer == true)
        {
            yield return new WaitUntil(() => isAnswer == true); // �亯 ��ư Ŭ������ ����
        }
        else
        {
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space)); // ��Ŭ�� �Ǵ� �����̽� �� ��ư���� ����
        }
        isAnswer = false;
        answerButton.SetActive(false);
        DisplayNext(index, number);
        nextImage.SetActive(false);
    }

    public bool dialog_read(int check_index)          // �ش� index�� ��ũ��Ʈ �κ��� �о����� Ȯ���ϴ� �Լ�
    {
        if (!dialog_cycles[check_index].check_cycle_read)
        {
            return true;
        }

        return false;
    }

    public void OnButtonClick(GameObject button)
    {
        switch (button.name)
        {
            case "AnswerButton":
                isAnswer = true;
                break;
        }
    }

}
