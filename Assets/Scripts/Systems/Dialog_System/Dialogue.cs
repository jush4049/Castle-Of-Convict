using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class Dialogue_info
{
    public string name;       // ��ȭ�� ȭ��
    [TextArea(3, 5)]
    public string content;    // ��ȭ ����
    public bool check_read;   // ���� ��ȭ�� �о����� Ȯ��
    public bool check_answer; // �亯�� ��������
    public string answer;     // �亯 ����
}

[System.Serializable]
public class Dialogue_cycle
{
    public string cycle_name;
    public List<Dialogue_info> info = new List<Dialogue_info>();
    public int cycle_index;
    public bool check_cycle_read;
}


public class Dialogue : MonoBehaviour
{
    [SerializeField]
    public static Dialogue instance = null;
    public List<Dialogue_cycle> dialogue_cycles = new List<Dialogue_cycle>(); // ��ȭ ���� �׷�
    public Queue<string> text_seq = new Queue<string>();                // ��ȭ �������� ������ ť�� �����Ѵ�.(������ ���� �Ǵ��ϱ� ����)
    public string name_;                                                // �ӽ÷� ������ ��ȭ ������ ����
    public string text_;                                                

    public TMP_Text nameText;                                            // ��ȭ�� ȭ��
    public TMP_Text dialogueText;                                          // ��ȭ ���� ����
    public TMP_Text answerText;                                          // ��ȭ�� ���� �亯 
    public GameObject dialogueCanvas;                                      // ��ȭ ���� ������Ʈ
    public GameObject answerButton;                                      // ���� ��ȭ ��¿� ��ư
    IEnumerator seq_;
    IEnumerator skip_seq;

    private float delay = 0.1f;                                          // ��ȭ ��� ������
    public bool dialogueRunning = false;                                   // ��ȭ ��� ����

    public GameObject mainUI;
    public GameObject nextImage;

    public bool isAnswer = false;

    public int readIndex;

    void Awake()
    {
        answerButton.SetActive(false);
        dialogueCanvas.SetActive(false);
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
        IEnumerator dialogue_co = dialogue_system_start(0);
        StartCoroutine(dialogue_co);
    }

    public void skip(int index)
    {
        index = DialogueControl.currentIndex;
        Debug.Log("���� ���̾�α�index : " + index);
        Debug.Log("readIndex : " + readIndex);
        Debug.Log(dialogue_cycles[index].info.Count);
        StopAllCoroutines();                                // ��� �ڷ�ƾ ��Ȱ��ȭ
        dialogueCanvas.SetActive(false);                      // ���̾�α� UI ��Ȱ��ȭ
        dialogue_cycles[index].check_cycle_read = true;       // ���� ���̾�α� ���� ó��
        dialogueRunning = false;                              // ���� ���� �ƴ�
        mainUI.SetActive(true);

        //dialogue_cycles[index].info[readIndex - 1].check_read = true;
        for (int i = readIndex; i < dialogue_cycles[index].info.Count; i++)       // ��ȭ ������ ������� ���
        {
            nameText.text = dialogue_cycles[index].info[i].name;
            text_ = text_seq.Dequeue();                                  // ��ȭ ������ pop

            dialogue_cycles[index].info[i].check_read = true;
        }
        readIndex = 1;
    }

    public IEnumerator dialogue_system_start(int index)               // ���̾�α� ��� ����
    {
        answerButton.SetActive(false);
        nameText = dialogueCanvas.GetComponent<parameter>().characterName;   // ������Ʈ���� �� ���� �޾ƿ���
        dialogueText = dialogueCanvas.GetComponent<parameter>().dialog;
        answerText = dialogueCanvas.GetComponent<parameter>().answer;

        Cursor.visible = true;
        dialogueRunning = true;
        mainUI.SetActive(false);
        foreach (Dialogue_info dialogue_temp in dialogue_cycles[index].info)  // ��ȭ ������ ť�� �����ϱ� ���� �ִ´�.
        {
            Debug.Log("ǥ���� ���̾�α� ����" + index);
            Debug.Log(dialogue_temp.content);
            text_seq.Enqueue(dialogue_temp.content);                      // Enqueue > ť�� ������ �߰�
        }

        dialogueCanvas.gameObject.SetActive(true);

        for (int i = 0; i < dialogue_cycles[index].info.Count; i++)       // ��ȭ ������ ������� ���
        {
            nameText.text = dialogue_cycles[index].info[i].name;
            // text_ = dialogue_cycles[index].info[i].content;
            text_ = text_seq.Dequeue();                                  // ��ȭ ������ pop

            seq_ = seq_sentence(index, i);                               // ��ȭ ���� ��� �ڷ�ƾ
            StartCoroutine(seq_);                                        // �ڷ�ƾ ����


            yield return new WaitUntil(() =>                            // �۾��� �Ϸ�� ������ ��ٸ�
            {
                if (dialogue_cycles[index].info[i].check_read)          // ���� ��ȭ�� �о����� �ƴ���
                {
                    return true;                                        // �о��ٸ� ����
                }
                else
                {
                    return false;                                       // ���� �ʾҴٸ� �ٽ� �˻�
                }
            });
        }
        dialogue_cycles[index].check_cycle_read = true;                 // �ش� ��ȭ �׷� ����
        dialogueRunning = false;
    }

    public void DisplayNext(int index, int number)                      // ���� �������� �Ѿ��
    {
        readIndex++;
        if (text_seq.Count == 0)                                        // ���� ������ ���ٸ�
        {
            dialogueCanvas.gameObject.SetActive(false);                 // ���̾�α� ����
            mainUI.SetActive(true);                                     // ���� UI Ȱ��ȭ
            readIndex = 1;
        }
        StopCoroutine(seq_);                                            // �������� �ڷ�ƾ ����
        dialogue_cycles[index].info[number].check_read = true;          // ���� ���� �������� ǥ��
    }

    public IEnumerator seq_sentence(int index, int number)              // ���� �ؽ�Ʈ �ѱ��� �� ���� ���
    {
        bool isAddingRichTextTag = false;                               // Rich Text Ȯ�� ����

        skip_seq = touch_wait(seq_, index, number);                     // ��ġ ��ŵ�� ���� ��ġ ��� �ڷ�ƾ �Ҵ�
        StartCoroutine(skip_seq);                                       // ��ġ ��� �ڷ�ƾ ����
        dialogueText.text = "";                                         // ��ȭ ���� �ʱ�ȭ
        foreach (char letter in text_.ToCharArray())                    // ��ȭ ���� �ѱ��� �� �̾Ƴ���
        {
            if (letter == '<' || isAddingRichTextTag)                   // Rich Text �˻�
            {
                isAddingRichTextTag = true;
                dialogueText.text += letter;
                if (letter == '>')
                {
                    isAddingRichTextTag = false;
                }
            }
            else
            {
                dialogueText.text += letter;                            // �ѱ��ھ� ���
                yield return new WaitForSeconds(delay);                 // ��� ������
            }
        }
        StopCoroutine(skip_seq);                                        // ���� ����� ������ ��ġ ��� �ڷ�ƾ ����
        nextImage.SetActive(true);

        if (dialogue_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialogue_cycles[index].info[number].answer;
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
        dialogueText.text = text_;                                                                         // ��� ���� �ѹ��� ���
        nextImage.SetActive(true);
        if (dialogue_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialogue_cycles[index].info[number].answer;
        }
        IEnumerator next = next_touch(index, number);                                                    // ��ȭ ���� �ڷ�ƾ�� ���� �Ǿ� ���� �������� ���� �ڷ�ƾ�� ����
        StartCoroutine(next);
    }

    public IEnumerator next_touch(int index, int number)                                                 // ���� �������� �Ѿ�� �ڷ�ƾ
    {
        StopCoroutine(seq_);
        StopCoroutine(skip_seq);
        yield return new WaitForSeconds(0.3f);
        if (dialogue_cycles[index].info[number].check_answer == true)
        {
            yield return new WaitUntil(() => isAnswer == true);                                           // �亯 ��ư Ŭ������ ����
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

    public bool dialogue_read(int check_index)          // �ش� index�� ��ũ��Ʈ �κ��� �о����� Ȯ���ϴ� �Լ�
    {
        if (!dialogue_cycles[check_index].check_cycle_read)
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
