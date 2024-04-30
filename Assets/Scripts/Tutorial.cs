using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tutorial : MonoBehaviour
{
    [SerializeField]
    private int currentPage = 0;        // ���� ������
    [SerializeField]
    private int minPage = 0;            // �ּ� ������
    [SerializeField]
    private int maxPage;                // �ִ� ������

    public GameObject[] pageList;       // UI ������
    public Image[] pageDot;             // �ϴ� ������ ǥ�� UI

    void Awake()
    {
        maxPage = pageList.Length - 1;
        PageChange();
    }
    public void PrevPage()
    {
        if (currentPage == minPage)
        {
            currentPage = maxPage;
        }
        else
        {
            currentPage--;
        }
        PageChange();
    }
    public void NextPage()
    {
        if (currentPage == maxPage)
        {
            currentPage = minPage;
        }
        else
        {
            currentPage++;
        }
        PageChange();
    }

    void PageChange()
    {
        for (int i = minPage; i <= maxPage; i++)
        {
            pageDot[currentPage].color = Color.red;
            pageList[currentPage].SetActive(true);
            if (i != currentPage)
            {
                pageDot[i].color = Color.white;
                pageList[i].SetActive(false);
            }
        }
    }
}
