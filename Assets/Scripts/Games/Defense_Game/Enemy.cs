using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DefenseGame
{
    public class Enemy : MonoBehaviour
    {
        public string value; // ���� ������Ʈ �ݶ��̴� �±� ��
        bool isDie;

        private Animator animator;
        GameObject defenseManager;

        void Awake()
        {
            defenseManager = GameObject.Find("DefenseManager");
            animator = GetComponent<Animator>();
            isDie = false;
        }

        void Update()
        {
            if (!isDie)
            {
                transform.Translate(Vector3.forward * Time.deltaTime * DefenseManager.enemySpeed);
            }
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
                isDie = true;
                animator.SetBool("isEnemyDie", true);
            }
        }

        void Die()
        {
            Destroy(gameObject);
        }
    }
}

