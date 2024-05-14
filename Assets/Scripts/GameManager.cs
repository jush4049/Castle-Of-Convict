using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    GameObject player;
    Vector3 spawnPoint;                 // 메인 맵 스폰 포인트
    Vector3 speedRunSpawnPoint;         // 미니게임 맵 스폰 포인트
    Vector3 speedRunExitPoint;
    Vector3 defenseSpawnPoint;
    Vector3 defenseExitPoint;

    // Game State
    enum STATE { play, wait, respawn, clear, complete, gameOver};
    STATE state;

    GameObject menuPanel;
    GameObject guidePanel;
    GameObject inventoryPanel;

    public TMP_Text missionContentText;

    public static bool isMenu;           // 메뉴 UI 생성 여부
    public static bool isKey;            // 아이템 획득 여부
    public static bool isMiniGame;       // 미니게임 실행 여부

    public Transform mainCamera;
    public GameObject cinemachineCamera;

    public GameObject mainUI;
    public GameObject[] gameUI;

    void Awake()
    {
        StartGame();
        isMenu = false;
        isMiniGame = false;

        menuPanel = GameObject.Find("MenuPanel");
        guidePanel = GameObject.Find("GuidePanel");
        inventoryPanel = GameObject.Find("InventoryPanel");
        menuPanel.SetActive(false);
        guidePanel.SetActive(false);
        inventoryPanel.SetActive(false);
    }

    void StartGame()
    {
        // 주인공을 시작 지점으로 이동
        spawnPoint = GameObject.Find("StartPoint").transform.position;
        speedRunSpawnPoint = GameObject.Find("SpeedRunSpawnPoint").transform.position;
        speedRunExitPoint = GameObject.Find("SpeedRunExitPoint").transform.position;
        defenseSpawnPoint = GameObject.Find("DefenseSpawnPoint").transform.position;
        defenseExitPoint = GameObject.Find("DefenseExitPoint").transform.position;

        player = GameObject.Find("Player");
        player.transform.position = spawnPoint;
        isKey = false;
        // state = STATE.play;

    }

    void Update()
    {
        if (!Dialogue.instance.dialogue_read(0))
        {
            missionContentText.text = "앞으로 이동하여 의문의 사람에게 말을 건다";
        }

        if (!Dialogue.instance.dialogue_read(1))
        {
            missionContentText.text = "오른쪽 문으로 이동하여 도전을 시작한다.";
        }

        switch (state)
        {
            case STATE.play:
                break;
            case STATE.respawn:
                // 플레이어 리스폰
                break;
            case STATE.clear:
                // 스테이지 클리어
                break;
            case STATE.gameOver:
                // 게임 종료
                break;
        }
    }

    public void ObjectSetActiveTrue(GameObject obj)
    {
        obj.SetActive(true);
    }

    public void ObjectSetActiveFalse(GameObject obj)
    {
        obj.SetActive(false);
    }

    public void MiniGameStart(string gameName)
    {
        switch (gameName)
        {
            case "Puzzle":
                Cursor.visible = true;

                Time.timeScale = 1;
                isMiniGame = true;
                gameUI[0].SetActive(true);
                mainUI.SetActive(false);
                cinemachineCamera.SetActive(false);

                mainCamera.position = new Vector3(20, -850, -20);
                mainCamera.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                break;
            case "SpeedRun":
                Time.timeScale = 1;

                isMiniGame = true;
                gameUI[1].SetActive(true);
                mainUI.SetActive(false);

                //mainCamera.position = new Vector3(470, 7, 17);
                player.transform.position = speedRunSpawnPoint;
                break;
            case "Defense":
                Time.timeScale = 1;

                isMiniGame = true;
                gameUI[2].SetActive(true);
                mainUI.SetActive(false);
                cinemachineCamera.SetActive(false);

                mainCamera.position = new Vector3(-92, 8, -120);
                mainCamera.rotation = Quaternion.Euler(new Vector3(52, 0, 0));
                player.transform.position = defenseSpawnPoint;
                break;
        }
    }

    public void MiniGameExit(string gameName)
    {
        switch (gameName)
        {
            case "Puzzle":
                isMiniGame = false;
                gameUI[0].SetActive(false);
                mainUI.SetActive(true);
                cinemachineCamera.SetActive(true);
                break;
            case "SpeedRun":
                isMiniGame = false;
                gameUI[1].SetActive(false);
                mainUI.SetActive(true);
                player.transform.position = speedRunExitPoint;
                break;
            case "Defense":
                isMiniGame = false;
                gameUI[2].SetActive(false);
                mainUI.SetActive(true);
                player.transform.position = defenseExitPoint;
                cinemachineCamera.SetActive(true);
                break;
        }
    }

    public static void GetKey()
    {
        isKey = true;
    }

    public void GameQuit()
    {
        Application.Quit();
    }
}
