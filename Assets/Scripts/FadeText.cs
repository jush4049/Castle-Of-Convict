using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class FadeText : MonoBehaviour
{
    TextMeshProUGUI fadeText;
    public string[] mapName;

    void Awake()
    {
        fadeText = GetComponent<TextMeshProUGUI>();
        //StartCoroutine(FadeOn());
    }

    public void MapOn(int kind)
    {
        StopCoroutine(FadeOff());
        fadeText.text = mapName[kind];
        StartCoroutine(FadeOn());
    }

    public IEnumerator FadeOn() // ���İ� 0���� 1�� ��ȯ
    {
        fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 0);
        while (fadeText.color.a < 1.0f)
        {
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, fadeText.color.a + (Time.deltaTime / 2.0f));
            yield return null;
        }
        StartCoroutine(FadeOff());
    }

    public IEnumerator FadeOff()  // ���İ� 1���� 0���� ��ȯ
    {
        fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, 1);
        while (fadeText.color.a > 0.0f)
        {
            fadeText.color = new Color(fadeText.color.r, fadeText.color.g, fadeText.color.b, fadeText.color.a - (Time.deltaTime / 2.0f));
            yield return null;
        }
        StopCoroutine(FadeOff()); // �ڷ�ƾ ����
    }
}
