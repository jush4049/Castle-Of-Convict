using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public List<Item> items;

    [SerializeField]
    private Transform slotParent;
    [SerializeField]
    private ItemSlot[] itemSlot;

#if UNITY_EDITOR
    private void OnValidate() {
        itemSlot = slotParent.GetComponentsInChildren<ItemSlot>();
    }
#endif

    void Awake()
    {
        AddSlot();
    }

    public void AddSlot()
    {
        for (int i = 0; i < itemSlot.Length; i++)
        {
            itemSlot[i].item = items[0]; // �ӽ÷� ������ �� �ֱ�
        }
        /*for (; i < itemSlot.Length; i++)
        {
            itemSlot[i].item = null;
        }*/
    }

    public void AddItem(Item _item)
    {
        if (items.Count < itemSlot.Length)
        {
            items.Add(_item);
            AddSlot();
        }
        else
        {
            Debug.Log("������ ���� ��");
        }
    }

    void Update()
    {
        if (!Dialogue.instance.dialogueRunning && Dialogue.instance.dialogue_cycles[1].check_cycle_read && !GameManager.isKey)
        {
            Debug.Log("������ ����");
            itemSlot[0].item = items[1];
            GameManager.isKey = true;
        }
    }

    public void GetItem(string game)
    {
        switch (game)
        {
            case "Puzzle":
                itemSlot[1].item = items[2];
                break;
            case "SpeedRun":
                itemSlot[2].item = items[3];
                break;
            case "Defense":
                itemSlot[3].item = items[4];
                break;
        }
    }
}
