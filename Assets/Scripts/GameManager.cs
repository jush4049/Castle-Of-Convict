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

    GameObject menuPanel;
    GameObject guidePanel;
    GameObject inventoryPanel;

    public TMP_Text missionContentText;
    public TMP_Text delivererText;

    bool isInfo;                         // 텍스트 수정 여부
    public static bool isMenu;           // 메뉴 UI 생성 여부
    public static bool isKey;            // 아이템 획득 여부
    public static bool isMiniGame;       // 미니게임 실행 여부
    public static bool isGameClear;      // 모든 미니게임 클리어 여부

    public Transform mainCamera;
    public GameObject cinemachineCamera;

    public GameObject mainUI;
    public GameObject[] gameUI;

    public Transform bgm; // 미니게임 BGM

    void Awake()
    {
        StartGame();
        isMenu = false;
        isMiniGame = false;
        isInfo = false;
        isGameClear = false;

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

    }

    void Update()
    {
        if (!Dialogue.instance.dialogue_read(0))
        {
            isInfo = true;
            if (isInfo)
            {
                missionContentText.text = "앞으로 이동하여 의문의 사람에게 말을 건다";
            }
            isInfo = false;
        }

        if (!Dialogue.instance.dialogue_read(1))
        {
            isInfo = true;
            if (isInfo)
            {
                missionContentText.text = "오른쪽 문으로 이동하여 도전을 시작한다.";
                delivererText.text = "인도자";
            }
            isInfo = false;
        }

        if (GameManager.isGameClear)
        {
            isInfo = true;
            if (isInfo)
            {
                missionContentText.text = "빛나는 문으로 이동하여 탈출한다.";
            }
            isInfo = false;
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
                bgm.SendMessage("BGMPlay", 2); // 미니게임 BGM
                break;
            case "SpeedRun":
                Time.timeScale = 1;

                isMiniGame = true;
                gameUI[1].SetActive(true);
                mainUI.SetActive(false);

                //mainCamera.position = new Vector3(470, 7, 17);
                player.transform.position = speedRunSpawnPoint;
                bgm.SendMessage("BGMPlay", 3); // 미니게임 BGM
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
                bgm.SendMessage("BGMPlay", 4); // 미니게임 BGM
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
                bgm.SendMessage("BGMPlay", 1); // 미니게임 BGM
                break;
            case "SpeedRun":
                isMiniGame = false;
                gameUI[1].SetActive(false);
                mainUI.SetActive(true);
                player.transform.position = speedRunExitPoint;
                bgm.SendMessage("BGMPlay", 1); // 미니게임 BGM
                break;
            case "Defense":
                isMiniGame = false;
                gameUI[2].SetActive(false);
                mainUI.SetActive(true);
                player.transform.position = defenseExitPoint;
                cinemachineCamera.SetActive(true);
                bgm.SendMessage("BGMPlay", 1); // 미니게임 BGM
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
