using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    float speed = 30;      // 타일 이동속도
    bool canMove = false;  // 이동 가능 여부
    Vector3 target;        // 목적지

    void Update()
    {
        if (canMove) MoveTile();
    }

    void MoveTile()
    {
        Vector3 pos = transform.position;                                    // 타일의 현재 위치
        pos = Vector3.MoveTowards(pos, target, speed * Time.deltaTime);      // 이동
        transform.position = pos;

        // 목적지와 근접하면 이동 종료
        if (Vector3.Distance(pos, target) < 0.05f)
        {
            transform.position = target;                                      // 목적지까지 이동

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

}
