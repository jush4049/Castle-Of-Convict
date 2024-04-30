using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PlayerControl : MonoBehaviour
{
    private Animator animator;
    private new Camera camera;

    private float speed = 7.0f;
    private float jumpForce = 7.0f;
    private Rigidbody rb;

    //private float gravity = -9.8f;
    //public float rotateSpeed = 10.0f;       // 회전 속도

    private bool isGrounded;
    private bool isMove;
    private bool isDie;
    private int slash;

    public LayerMask groundLayer;
    private float groundCheckDistance = 0.1f;

    GameObject respawnPoint;
    GameObject speedRunRespawnPoint;
    public Transform sfx;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;

        isMove = animator.GetBool("isMove");
        isDie = animator.GetBool("isDie");
        slash = animator.GetInteger("slash");

        respawnPoint = GameObject.Find("RespawnPoint");
        speedRunRespawnPoint = GameObject.Find("SpeedRunSpawnPoint");
    }

    void Update()
    {
        if (!isDie && !Dialog.instance.dialogRunning && !GameManager.isMenu && !GameManager.isMiniGame)
        {
            float h = Input.GetAxis("Horizontal");
            float v = Input.GetAxis("Vertical");
            Move(h, v);

            Jump();
            Attack();
            //Cursor.visible = false;
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Debug.Log("알트");
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
        }

        if (GaugeBar.time < 0)
        {
            Die();
        }
    }

    void FixedUpdate()
    {

    }

    private void Move(float h, float v)
    {
        if (h != 0 || v != 0)
        {
            animator.SetBool("isMove", true);
            animator.SetInteger("slash", 0);
            // 카메라 방향 확인
            Vector3 cameraForward = camera.transform.forward;
            Vector3 cameraRight = camera.transform.right;
            cameraForward.y = 0.0f;
            cameraRight.y = 0.0f;
            Vector3 moveDir = (cameraForward.normalized * v) + (cameraRight.normalized * h);

            // 카메라 방향 기준 이동
            transform.position += moveDir * speed * Time.deltaTime;
            // rb.MovePosition(rb.position + moveDir *speed * Time.fixedDeltaTime);

            //  방향으로 바라보기
            transform.localRotation = Quaternion.LookRotation(moveDir);
        }
        else
        {
            animator.SetBool("isMove", false);
        }
    }

    private void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetBool("isJump", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
        }
        else
        {
            animator.SetBool("isJump", false);
        }
    }

    IEnumerator Slash()
    {
        animator.SetInteger("slash", 1);
        yield return new WaitForSeconds(0.6f);
        animator.SetInteger("slash", 0);
    }

    private void Attack()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StartCoroutine(Slash());
        }
    }

    void SfxSound(int kind)
    {
        sfx.SendMessage("PlaySound", kind);
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Fall")
        {
            transform.position = respawnPoint.transform.position;
        }
        if (col.gameObject.tag == "SpeedRun")
        {
            SpeedRunManager.isRepair = true;
            transform.position = speedRunRespawnPoint.transform.position;
            GaugeBar.time -= 10;
        }

        if (col.gameObject.tag == "Coin")
        {
            Debug.Log("zhdls");
            SfxSound(0);
        }
    }

    private void Die()
    {
        animator.SetBool("isDie", true);
        isDie = true;
    }

    public void Revive()
    {
        Debug.Log("부활!");
        animator.SetBool("isDie", false);
        isDie = false;
        animator.SetBool("isMove", true);
    }
}
