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

    public static bool isDefenseGame;        // ���� ���� ����
    private float timerMax = 60;
    private int heartMax = 3;
    private int EnemyMax;

    int heart;                                  // ���� HP
    float timerNum;                          // Ÿ�̸�
    float respawnTime;                       // ������ �ʱ�ȭ �ð�
    public GameObject[] enemyList;           // ����
    public Transform[] enemyRespawnPoint;    // ���� ������ ����Ʈ
    public TMP_Text timerText;               // Ÿ�̸� UI
    public GameObject[] heartImage;          // ��Ʈ �̹���(hp)

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
            timerText.text = string.Format("{0:N2}", timerNum); // �Ҽ��� �� ��° �ڸ����� ǥ��
        }

        Debug.Log(heart);

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
        foreach (Transform child in enemyParent.transform) // ������ ���� ����Ʈ�� ��� �ڽ� ������Ʈ ����
        {
            Destroy(child.gameObject);
        }
        heart = heartMax;                                        // ���� ü�� �ʱ�ȭ
        timerText.text = timerMax.ToString();              // UI ǥ�� �ʱ�ȭ
        timerNum = timerMax;                               // ���� Ÿ�̸� ���� �ʱ�ȭ

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
            if (timerNum < 30)                                    // Ÿ�̸� �ð��� ���� �ð� ���ϸ� ������ �ڷ�ƾ ����
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
        StopCoroutine(Respawn());
        foreach (Transform child in enemyParent.transform) // ������ ���� ����Ʈ�� ��� �ڽ� ������Ʈ ����
        {
            Destroy(child.gameObject);
        }
        GameManager.isMiniGame = true;
        Cursor.visible = true;
    }
}
