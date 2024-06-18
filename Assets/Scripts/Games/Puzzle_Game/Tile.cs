using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class Tile : MonoBehaviour, IPointerDownHandler
{
    float speed = 30;      // Ÿ�� �̵��ӵ�
    bool canMove = false;  // �̵� ���� ����
    Vector3 target;        // ������

    AudioSource audioSource; // ���� �̵� ����

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (canMove) MoveTile();
    }

    void MoveTile()
    {
        Vector3 pos = transform.position;                                    // Ÿ���� ���� ��ġ
        pos = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);      // �̵�
        transform.position = pos;

        // �������� �����ϸ� �̵� ����
        if (Vector3.Distance(pos, target) < 0.05f)
        {
            transform.position = target;                                      // ���������� �̵�

            GameObject.Find("PuzzleManager").SendMessage("SetCalculation");
            canMove = false;
        }
    }

    void SetMove(Vector3 _target)
    {
        target = _target;
        canMove = true;
    }

    void OnMouseDown()
    {
        int n = int.Parse(name.Substring(4));
        GameObject.Find("PuzzleManager").SendMessage("SetTouch", n);
    }

    void IPointerDownHandler.OnPointerDown(PointerEventData eventData)
    {
        audioSource.Play();
    }
}
