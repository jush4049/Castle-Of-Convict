using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GaugeBar : MonoBehaviour
{
    RectTransform gaugeBar;
    Rect rect;

    public Image guageBarImage;

    public static float time = 10;
    float min = 0;
    float max = 10;

    void Awake()
    {
        time = 10;
    }

    void Start()
    {
        var back = transform.Find("GaugeBar_Background").GetComponent<RectTransform>();
        rect = back.rect;

        gaugeBar = transform.Find("GaugeBar").GetComponent<RectTransform>();
        gaugeBar.sizeDelta = new Vector2(rect.width, rect.height);
    }

    void Update()
    {
        time = Mathf.Clamp(time, min, max);
        if (time > min)
        {
            time -= Time.deltaTime;
            gaugeBar.sizeDelta = new Vector2(rect.width * time * 0.1f, rect.height);
            if (time >= 3)
            {
                guageBarImage.color = Color.green;
            }
            if (time < 3)
            {   
                guageBarImage.color = Color.red;
            }
        }
    }
}
