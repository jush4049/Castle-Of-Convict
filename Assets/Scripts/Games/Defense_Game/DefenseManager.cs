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
    GameObject startButton;

    public static bool isDefenseGame;        // ���� ���� ����
    public static float enemySpeed;                 // ���� �ӵ� ��
    private float timerMax = 60;
    private int heartMax = 3;
    private int EnemyMax;

    int heart;                               // ���� HP
    float timerNum;                          // Ÿ�̸�
    float respawnTime;                       // ������ �ʱ�ȭ �ð�
    public GameObject[] enemyList;           // ����
    public Transform[] enemyRespawnPoint;    // ���� ������ ����Ʈ
    public GameObject[] heartImage;          // ��Ʈ �̹���(hp)
    public TMP_Text timerText;               // Ÿ�̸� UI
    public TMP_Text infoText;                // Ÿ�̸� UI

    GameObject enemyParent;

    void Awake()
    {
        isDefenseGame = false;
        startUI = GameObject.Find("DefenseGameStartPanel");
        gameUI = GameObject.Find("DefenseGameUI");
        clearPanel = GameObject.Find("DefenseGameClearPanel");
        restartPanel = GameObject.Find("DefenseGameRestartPanel");
        startButton = GameObject.Find("DefenseGameStartButton");

        startUI.SetActive(false);
        gameUI.SetActive(false);
        clearPanel.SetActive(false);
        restartPanel.SetActive(false);
        enemyParent = GameObject.Find("EnemyList");
        respawnTime = 3.0f;
        infoText.text = "";

        enemySpeed = 2.0f;
        DefenseInit();
    }

    void Update()
    {
        if (isDefenseGame)
        {
            timerNum -= Time.deltaTime;
            timerText.text = string.Format("{0:N2}", timerNum); // �Ҽ��� �� ��° �ڸ����� ǥ��
        }

        // Debug.Log(heart);

        if (heart <= 0 && isDefenseGame)                  // ���� �й�
        {
            GameOver();
            isDefenseGame = false;
        }
        if (timerNum <= 0 && isDefenseGame)               // ���� �¸�
        {
            GameClear();
            isDefenseGame = false;
        }
    }

    // �ʱ�ȭ
    public void DefenseInit()
    {
        foreach (Transform child in enemyParent.transform)       // ������ ���� ����Ʈ�� ��� �ڽ� ������Ʈ ����
        {
            Destroy(child.gameObject);
        }
        heart = heartMax;                                        // ���� ü�� �ʱ�ȭ
        timerText.text = timerMax.ToString();                    // UI ǥ�� �ʱ�ȭ
        timerNum = timerMax;                                     // ���� Ÿ�̸� ���� �ʱ�ȭ
        infoText.text = "";                                      // �˸� �ؽ�Ʈ �ʱ�ȭ
        for (int i = 0; i < heartMax; i++)
        {
            heartImage[i].SetActive(true);
        }
        startButton.SetActive(true);
    }

    public void DefenseStart()
    {
        isDefenseGame = true;
        DefenseInit();
        GameManager.isMiniGame = false;
        StartCoroutine("Respawn");
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
            if (timerNum < 30 && timerNum > 27)                     // Ÿ�̸� �ð��� ���� �ð� ���ϸ� ������ �ڷ�ƾ ���� (���� ������ �ӵ� ����)
            {
                respawnTime = 1.0f;
                enemySpeed = 4.0f;
                infoText.text = "���� ������� �ӵ��� �������ϴ�!";
            }
            else if (timerNum < 30)
            {
                respawnTime = 1.0f;
                enemySpeed = 4.0f;
                infoText.text = "";
            }
            else
            {
                respawnTime = 3.0f;
                enemySpeed = 2.0f;
                infoText.text = "";
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
        StopCoroutine("Respawn");
        foreach (Transform child in enemyParent.transform) // ������ ���� ����Ʈ�� ��� �ڽ� ������Ʈ ����
        {
            Destroy(child.gameObject);
        }
        GameManager.isMiniGame = true;
        Cursor.visible = true;
    }

    private void GameClear()
    {
        clearPanel.SetActive(true);
        StopCoroutine("Respawn");
        foreach (Transform child in enemyParent.transform) // ������ ���� ����Ʈ�� ��� �ڽ� ������Ʈ ����
        {
            Destroy(child.gameObject);
        }
        GameManager.isMiniGame = true;
        Cursor.visible = true;
    }
}
