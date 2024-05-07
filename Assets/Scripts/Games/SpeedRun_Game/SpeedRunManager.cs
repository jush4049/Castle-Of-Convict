using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SpeedRunManager : MonoBehaviour
{
    GameObject startUI;      // ���� UI
    GameObject gameUI;
    GameObject clearPanel;
    GameObject gauge;
    GameObject timer;
    GameObject restartPanel;
    GameObject startButton;

    [SerializeField]
    List<GameObject> coinList;   // ������Ʈ(����) ����Ʈ
    public GameObject _coinList;
    int coinMaxCount;            // ����Ʈ�� ũ��

    public TMP_Text timerText;
    public TMP_Text CoinCountText;

    bool isGameOver;                // ���� ���� ����
    bool isGameClear;               // ���� Ŭ���� ����
    public static bool isRepair;    // �� ���� ����
    public static int coinCount;    // ���� ī��Ʈ

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

        if (GaugeBar.time < 0 && !isGameOver)           // Ŭ���� ���� ����
        {
            isGameOver = true;
            Invoke("GameOver", 5.0f);
        }

        if (!isGameClear && coinCount == coinMaxCount)  // Ŭ���� ���� ����
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

    public void SpeedRunInit()                          // ���� ���� �ʱ�ȭ
    {
        GaugeBar.time = 10;
        coinCount = 0;
        restartPanel.SetActive(false);
        gauge.SetActive(false);
        isGameOver = false;
    }

    public void SpeedRunStart()                          // ���� ����
    {
        SpeedRunInit();
        StartCoroutine(Timer());
        for (int i = 0; i < coinMaxCount; i++)
        {
            coinList[i].SetActive(true);
        }
        clearPanel.SetActive(false);
    }

    public void SpeedRunExit()                           // ���� ����
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
        timerText.color = Color.white;                    // Ÿ�̸� ��ũ��Ʈ
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
        CoinCountText.text = "���� ���� ȹ�� �� : " + coinCount.ToString() + " / " + coinMaxCount.ToString();
    }
}
