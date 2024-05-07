using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DefenseManager : MonoBehaviour
{
    GameObject startUI;
    GameObject gameUI;
    GameObject clearPanel;
    GameObject restartPanel;

    public static bool isDefenseGame;        // 게임 실행 여부
    private float timerMax = 60;
    private int heartMax = 3;
    private int EnemyMax;

    int heart;                                  // 기지 HP
    float timerNum;                          // 타이머
    float respawnTime;                       // 리스폰 초기화 시간
    public GameObject[] enemyList;           // 적군
    public Transform[] enemyRespawnPoint;    // 적군 리스폰 포인트
    public TMP_Text timerText;               // 타이머 UI
    public GameObject[] heartImage;          // 하트 이미지(hp)

    GameObject enemyParent;

    void Awake()
    {
        isDefenseGame = false;
        startUI = GameObject.Find("DefenseGameStartPanel");
        gameUI = GameObject.Find("DefenseGameUI");
        clearPanel = GameObject.Find("DefenseGameClearPanel");
        restartPanel = GameObject.Find("DefenseGameRestartPanel");

        startUI.SetActive(false);
        gameUI.SetActive(false);
        clearPanel.SetActive(false);
        restartPanel.SetActive(false);
        enemyParent = GameObject.Find("EnemyList");
        respawnTime = 3.0f;

        DefenseInit();
        //StartCoroutine(Respawn());
    }

    void Update()
    {
        if (isDefenseGame)
        {
            timerNum -= Time.deltaTime;
            timerText.text = string.Format("{0:N2}", timerNum); // 소수점 두 번째 자리까지 표시
        }

        Debug.Log(heart);

        if (heart <= 0 && isDefenseGame)                  // 게임 패배
        {
            GameOver();
            isDefenseGame = false;
        }

        if (timerNum <= 0 && isDefenseGame)               // 게임 승리
        {
            GameClear();
            isDefenseGame = false;
        }
    }

    // 초기화
    public void DefenseInit()
    {
        foreach (Transform child in enemyParent.transform) // 생성된 적군 리스트의 모든 자식 오브젝트 삭제
        {
            Destroy(child.gameObject);
        }
        heart = heartMax;                                        // 기지 체력 초기화
        timerText.text = timerMax.ToString();              // UI 표시 초기화
        timerNum = timerMax;                               // 실제 타이머 숫자 초기화

        for (int i = 0; i < heartMax; i++)
        {
            heartImage[i].SetActive(true);
        }
    }

    public void DefenseStart()
    {
        isDefenseGame = true;
        DefenseInit();
        GameManager.isMiniGame = false;
        StartCoroutine(Respawn());
    }

    private void EnemyCreateStart()
    {
        int enemyValue = Random.Range(0, 2);
        int enemyRespawnValue = Random.Range(0, 4);
        GameObject enemy = Instantiate(enemyList[enemyValue], enemyRespawnPoint[enemyRespawnValue].position, Quaternion.identity);
        enemy.transform.SetParent(enemyParent.transform);
        enemy.transform.eulerAngles = new Vector3(0, 180, 0);
    }
    
    IEnumerator Respawn()
    {
        while (true && isDefenseGame)
        {
            EnemyCreateStart();
            yield return new WaitForSeconds(respawnTime);
            if (timerNum < 30)                                    // 타이머 시간이 일정 시간 이하면 리스폰 코루틴 변경
            {
                respawnTime = 1;
            }
        }
    }

    public void DefenseExit()
    {
        isDefenseGame = false;
        DefenseInit();
    }

    void Damage(int damage)
    {
        heartImage[heart - 1].SetActive(false);
        heart -= damage;
    }

    private void GameOver()
    {
        restartPanel.SetActive(true);
        StopCoroutine(Respawn());
        foreach (Transform child in enemyParent.transform) // 생성된 적군 리스트의 모든 자식 오브젝트 삭제
        {
            Destroy(child.gameObject);
        }
        GameManager.isMiniGame = true;
        Cursor.visible = true;
    }

    private void GameClear()
    {
        clearPanel.SetActive(true);
        StopCoroutine(Respawn());
        foreach (Transform child in enemyParent.transform) // 생성된 적군 리스트의 모든 자식 오브젝트 삭제
        {
            Destroy(child.gameObject);
        }
        GameManager.isMiniGame = true;
        Cursor.visible = true;
    }
}
