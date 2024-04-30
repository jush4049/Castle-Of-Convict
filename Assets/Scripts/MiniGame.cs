using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MiniGame : MonoBehaviour
{
    public GameObject miniGameStartPanel;
    
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            GameManager.isMiniGame = true;
            Cursor.visible = true;
            miniGameStartPanel.SetActive(true);
        }
    }

    public void GameStart()
    {
        miniGameStartPanel.SetActive(false);
    }

    public void PanelExit()
    {
        GameManager.isMiniGame = false;
        Cursor.visible = false;
        miniGameStartPanel.SetActive(false);
    }
}
