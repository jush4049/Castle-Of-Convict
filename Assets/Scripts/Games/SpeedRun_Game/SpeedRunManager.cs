using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedRunManager : MonoBehaviour
{
    GameObject startUI;      // 게임 UI
    GameObject gameUI;
    GameObject clearPanel;
    GameObject gauge;
    GameObject timer;
    GameObject restartPanel;
    GameObject startButton;

    [SerializeField]
    List<GameObject> coinList;   // 오브젝트(코인) 리스트
    public GameObject _coinList;
    int coinMaxCount;            // 리스트의 크기

    public TMP_Text timerText;
    public TMP_Text CoinCountText;

    bool isGameOver;                // 게임 오버 여부
    bool isGameClear;               // 게임 클리어 여부
    public static bool isRepair;    // 맵 복구 여부
    public static int coinCount;    // 코인 카운트

    void Awake()
    {
        coinMaxCount = _coinList.transform.childCount;
        for (int i = 0; i < coinMaxCount; i++)
        {
            coinList.Add(_coinList.transform.GetChild(i).gameObject);
        }

        isGameOver = false;
        isGameClear = false;
        isRepair = false;
        coinCount = 0;
        CoinCheck();

        startUI = GameObject.Find("SpeedRunGameStartPanel");
        gameUI = GameObject.Find("SpeedRunGameUI");
        clearPanel = GameObject.Find("SpeedRunGameClearPanel");
        gauge = GameObject.Find("Gauge");
        timer = GameObject.Find("TimerText");
        restartPanel = GameObject.Find("SpeedRunGameRestartPanel");
        startButton = GameObject.Find("SpeedRunGameStartButton");

        gauge.SetActive(false);
        startUI.SetActive(false);
        gameUI.SetActive(false);
        clearPanel.SetActive(false);
        timer.SetActive(false);
        restartPanel.SetActive(false);
    }

    void Update()
    {
        CoinCheck();

        if (GaugeBar.time < 0 && !isGameOver)           // 클리어 실패 조건
        {
            isGameOver = true;
            Invoke("GameOver", 5.0f);
        }

        if (!isGameClear && coinCount == coinMaxCount)  // 클리어 성공 조건
        {
            GameManager.isMiniGame = true;
            GameClear();
            isGameClear = true;
            Cursor.visible = true;
        }
    }

    public void GameStop(int value)
    {
        Time.timeScale = value;
    }

    public void SpeedRunInit()                          // 게임 세팅 초기화
    {
        GaugeBar.time = 10;
        coinCount = 0;
        restartPanel.SetActive(false);
        gauge.SetActive(false);
        isGameOver = false;
    }

    public void SpeedRunStart()                          // 게임 시작
    {
        SpeedRunInit();
        StartCoroutine(Timer());
        for (int i = 0; i < coinMaxCount; i++)
        {
            coinList[i].SetActive(true);
        }
        clearPanel.SetActive(false);
    }

    public void SpeedRunExit()                           // 게임 퇴장
    {
        SpeedRunInit();
        isGameClear = false;
        //startButton.SetActive(true);
        for (int i = 0; i < coinMaxCount; i++)
        {
            coinList[i].SetActive(true);
        }
        clearPanel.SetActive(false);
    }

    IEnumerator Timer()
    {
        gauge.SetActive(false);
        restartPanel.SetActive(false);
        GameManager.isMiniGame = true;
        timer.SetActive(true);
        timerText.color = Color.white;                    // 타이머 스크립트
        timerText.text = "3";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "2";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.text = "1";
        yield return new WaitForSecondsRealtime(1.0f);
        timerText.color = Color.green;
        timerText.text = "Go!";
        GameManager.isMiniGame = false;
        gauge.SetActive(true);
        yield return new WaitForSecondsRealtime(1.0f);
        timer.SetActive(false);
    }

    private void GameOver()
    {
        Cursor.visible = true;
        restartPanel.SetActive(true);
        isGameOver = true;
    }

    private void GameClear()
    {
        gauge.SetActive(false);
        clearPanel.SetActive(true);
    }

    private void CoinCheck()
    {
        CoinCountText.text = "현재 코인 획득 수 : " + coinCount.ToString() + " / " + coinMaxCount.ToString();
    }
}
