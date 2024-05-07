using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefenseGame
{
    public class Enemy : MonoBehaviour
    {
        public string value; // ���� ������Ʈ �ݶ��̴� �±� ��
        float speed;         // ���� �ӵ�

        private Animator animator;
        GameObject defenseManager;

        void Awake()
        {
            defenseManager = GameObject.Find("DefenseManager");
            animator = GetComponent<Animator>();
        }

        void Start ()
        {
            speed = 2.0f;
        }

        void Update()
        {
            transform.Translate(Vector3.forward * Time.deltaTime * speed);
        }

        private void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Base")
            {
                Destroy(gameObject);
                defenseManager.SendMessage("Damage", 1);
            }

            if (col.gameObject.tag == value) // ���� ���� ���ϴ� ������ �ٸ�
            {
                Invoke("Die", 3f);
                speed = 0;
                animator.SetBool("isEnemyDie", true);
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }
}

