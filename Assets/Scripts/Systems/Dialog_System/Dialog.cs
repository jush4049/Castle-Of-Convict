using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
[System.Serializable]
public class Dialog_info
{
    public string name;       // 대화의 화자
    [TextArea(3, 5)]
    public string content;    // 대화 지문
    public bool check_read;   // 현재 대화를 읽었는지 확인
    public bool check_answer; // 답변의 존재유무
    public string answer;     // 답변 내용
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
    public List<Dialog_cycle> dialog_cycles = new List<Dialog_cycle>(); // 대화 지문 그룹
    public Queue<string> text_seq = new Queue<string>();                // 대화 지문들의 내용을 큐로 저장한다.(끝점을 쉽게 판단하기 위해)
    public string name_;                                                // 임시로 저장할 대화 지문의 내용
    public string text_;                                                

    public TMP_Text nameText;                                            // 대화의 화자
    public TMP_Text dialogText;                                          // 대화 지문 내용
    public TMP_Text answerText;                                          // 대화에 대한 답변 
    public GameObject dialogCanvas;                                      // 대화 지문 오브젝트
    public GameObject answerButton;                                      // 다음 대화 출력용 버튼
    IEnumerator seq_;
    IEnumerator skip_seq;

    private float delay = 0.1f;                                          // 대화 출력 딜레이
    public bool dialogRunning = false;                                   // 대화 출력 여부

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
        Debug.Log("현재 다이얼로그index : " + index);
        Debug.Log("readIndex : " + readIndex);
        Debug.Log(dialog_cycles[index].info.Count);
        StopAllCoroutines();                                // 모든 코루틴 비활성화
        dialogCanvas.SetActive(false);                      // 다이얼로그 UI 비활성화
        dialog_cycles[index].check_cycle_read = true;       // 현재 다이얼로그 읽음 처리
        dialogRunning = false;                              // 동작 중이 아님
        mainUI.SetActive(true);

        //dialog_cycles[index].info[readIndex - 1].check_read = true;
        for (int i = readIndex; i < dialog_cycles[index].info.Count; i++)       // 대화 단위를 순서대로 출력
        {
            nameText.text = dialog_cycles[index].info[i].name;
            text_ = text_seq.Dequeue();                                  // 대화 지문을 pop

            dialog_cycles[index].info[i].check_read = true;
        }
        readIndex = 1;
    }

    public IEnumerator dialog_system_start(int index)               // 다이얼로그 출력 시작
    {
        answerButton.SetActive(false);
        nameText = dialogCanvas.GetComponent<parameter>().characterName;   // 오브젝트에서 각 변수 받아오기
        dialogText = dialogCanvas.GetComponent<parameter>().dialog;
        answerText = dialogCanvas.GetComponent<parameter>().answer;

        Cursor.visible = true;
        dialogRunning = true;
        mainUI.SetActive(false);
        foreach (Dialog_info dialog_temp in dialog_cycles[index].info)  // 대화 단위를 큐로 관리하기 위해 넣는다.
        {
            Debug.Log("표시할 다이얼로그 순서" + index);
            Debug.Log(dialog_temp.content);
            text_seq.Enqueue(dialog_temp.content);                      // Enqueue > 큐에 데이터 추가
        }

        dialogCanvas.gameObject.SetActive(true);

        for (int i = 0; i < dialog_cycles[index].info.Count; i++)       // 대화 단위를 순서대로 출력
        {
            nameText.text = dialog_cycles[index].info[i].name;
            // text_ = dialog_cycles[index].info[i].content;
            text_ = text_seq.Dequeue();                                  // 대화 지문을 pop

            seq_ = seq_sentence(index, i);                               // 대화 지문 출력 코루틴
            StartCoroutine(seq_);                                        // 코루틴 실행


            yield return new WaitUntil(() =>                            // 작업이 완료될 때까지 기다림
            {
                if (dialog_cycles[index].info[i].check_read)            // 현재 대화를 읽었는지 아닌지
                {
                    return true;                                        // 읽었다면 진행
                }
                else
                {
                    return false;                                       // 읽지 않았다면 다시 검사
                }
            });
        }
        dialog_cycles[index].check_cycle_read = true;                   // 해당 대화 그룹 읽음
        dialogRunning = false;
    }

    public void DisplayNext(int index, int number)                      // 다음 지문으로 넘어가기
    {
        readIndex++;
        if (text_seq.Count == 0)                                        // 다음 지문이 없다면
        {
            dialogCanvas.gameObject.SetActive(false);                   // 다이얼로그 끄기
            mainUI.SetActive(true);                                     // 메인 UI 활성화
            readIndex = 1;
        }
        StopCoroutine(seq_);                                            // 실행중인 코루틴 종료
        dialog_cycles[index].info[number].check_read = true;            // 현재 지문 읽음으로 표시
    }

    public IEnumerator seq_sentence(int index, int number)              // 지문 텍스트 한글자 씩 연속 출력
    {
        skip_seq = touch_wait(seq_, index, number);                     // 터치 스킵을 위한 터치 대기 코루틴 할당
        StartCoroutine(skip_seq);                                       // 터치 대기 코루틴 시작
        dialogText.text = "";                                           // 대화 지문 초기화
        foreach (char letter in text_.ToCharArray())                    // 대화 지문 한글자 씩 뽑아내기
        {
            dialogText.text += letter;                                  // 한글자씩 출력
            yield return new WaitForSeconds(delay);                     // 출력 딜레이
        }
        StopCoroutine(skip_seq);                                        // 지문 출력이 끝나면 터치 대기 코루틴 해제
        nextImage.SetActive(true);

        if (dialog_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialog_cycles[index].info[number].answer;
            IEnumerator next = next_touch(index, number);                   // 버튼 이외에 부분을 터치해도 넘어가는 코루틴 시작
            StartCoroutine(next);
        }
        else
        {
            IEnumerator next = next_touch(index, number);
            StartCoroutine(next);
        }
    }

    public IEnumerator touch_wait(IEnumerator seq, int index, int number)                                // 터치 대기 코루틴
    {
        yield return new WaitForSeconds(0.3f);
        yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space));  // 좌클릭 또는 스페이스 바 버튼으로 진행
        StopCoroutine(seq);                                                                              // 대화 지문 코루틴 해제
        dialogText.text = text_;                                                                         // 모든 지문 한번에 출력
        nextImage.SetActive(true);
        if (dialog_cycles[index].info[number].check_answer == true)
        {
            answerButton.SetActive(true);
            answerText.text = dialog_cycles[index].info[number].answer;
        }
        IEnumerator next = next_touch(index, number);                                                 // 대화 지문 코루틴이 해제 되어 다음 지문으로 가는 코루틴을 시작
        StartCoroutine(next);
    }

    public IEnumerator next_touch(int index, int number)                                              // 다음 지문으로 넘어가는 코루틴
    {
        StopCoroutine(seq_);
        StopCoroutine(skip_seq);
        yield return new WaitForSeconds(0.3f);
        if (dialog_cycles[index].info[number].check_answer == true)
        {
            yield return new WaitUntil(() => isAnswer == true); // 답변 버튼 클릭으로 진행
        }
        else
        {
            yield return new WaitUntil(() => Input.GetMouseButtonUp(0) || Input.GetKeyDown(KeyCode.Space)); // 좌클릭 또는 스페이스 바 버튼으로 진행
        }
        isAnswer = false;
        answerButton.SetActive(false);
        DisplayNext(index, number);
        nextImage.SetActive(false);
    }

    public bool dialog_read(int check_index)          // 해당 index의 스크립트 부분을 읽었는지 확인하는 함수
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
