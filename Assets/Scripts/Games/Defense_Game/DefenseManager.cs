using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefenseManager : MonoBehaviour
{
    GameObject startUI;
    GameObject gameUI;
    GameObject clearPanel;

    void Awake()
    {
        startUI = GameObject.Find("DefenseGameStartPanel");
        gameUI = GameObject.Find("DefenseGameUI");
        clearPanel = GameObject.Find("DefenseGameClearPanel");
        startUI.SetActive(false);
        gameUI.SetActive(false);
        clearPanel.SetActive(false);
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
