using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public GameObject NpcDialog;
    public GameObject talkDialog;

    void Start()
    {
        NpcDialog.SetActive(false);
    }

    void Update()
    {
        if (Dialogue.instance.dialogueRunning)
        {
            talkDialog.SetActive(false);
        }
    }
    public void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Player")
        {
            NpcDialog.SetActive(true);
            talkDialog.SetActive(true);
        }
    }

    public void OnTriggerExit(Collider col)
    {
        NpcDialog.SetActive(false);
        talkDialog.SetActive(false);
    }
}
