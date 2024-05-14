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
    private bool isGuard;
    private int slash;

    public LayerMask groundLayer;
    private float groundCheckDistance = 0.1f;

    GameObject respawnPoint;
    GameObject speedRunRespawnPoint;
    GameObject defenseRespawnPoint;

    GameObject guardEffect;
    GameObject slashEffect;

    public Transform sfx;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        camera = Camera.main;
        slash = 0;

        isMove = animator.GetBool("isMove");
        isDie = animator.GetBool("isDie");
        slash = animator.GetInteger("slash");
        isGuard = animator.GetBool("isGuard");

        guardEffect = GameObject.Find("GuardEffect");
        slashEffect = GameObject.Find("SlashEffect");
        respawnPoint = GameObject.Find("RespawnPoint");
        speedRunRespawnPoint = GameObject.Find("SpeedRunSpawnPoint");
        defenseRespawnPoint = GameObject.Find("DefenseSpawnPoint");

        guardEffect.SetActive(false);
        slashEffect.SetActive(false);
    }

    void Update()
    {
        if (!isDie && !Dialogue.instance.dialogueRunning && !GameManager.isMenu && !GameManager.isMiniGame)
        {
            if (!isGuard && slash == 0)
            {
                float h = Input.GetAxis("Horizontal");
                float v = Input.GetAxis("Vertical");
                Move(h, v);
            }

            Jump();
            Action();
            if (Input.GetKey(KeyCode.LeftAlt))
            {
                Cursor.visible = true;
            }
            else
            {
                Cursor.visible = false;
            }
        }

        if (Dialogue.instance.dialogueRunning)
        {
            animator.SetBool("isMove", false);
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

            Vector3 cameraForward = camera.transform.forward;                                       // 카메라 방향 확인
            Vector3 cameraRight = camera.transform.right;
            cameraForward.y = 0.0f;
            cameraRight.y = 0.0f;
            Vector3 moveDir = (cameraForward.normalized * v) + (cameraRight.normalized * h);

            transform.position += moveDir * speed * Time.deltaTime;                                 // 카메라 방향 기준 이동
            // rb.MovePosition(rb.position + moveDir *speed * Time.fixedDeltaTime);

            transform.localRotation = Quaternion.LookRotation(moveDir);                             //  방향으로 바라보기
        }
        else
        {
            animator.SetBool("isMove", false);
        }
    }

    private void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);  // 바닥 레이어 확인

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

    private void Action()
    {
        // 베기
        if (Input.GetKey(KeyCode.E))   // 버튼 누르기
        {
            animator.SetInteger("slash", 1);
            slash = 1;
            slashEffect.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.E)) // 버튼 떼기
        {
            animator.SetInteger("slash", 0);
            slash = 0;
            slashEffect.SetActive(false);
        }

        // 가드
        if (Input.GetKey(KeyCode.Q))   // 버튼 누르기
        {
            animator.SetBool("isGuard", true);
            isGuard = true;
            guardEffect.SetActive(true);
        }
        if (Input.GetKeyUp(KeyCode.Q)) // 버튼 떼기
        {
            animator.SetBool("isGuard", false);
            isGuard = false;
            guardEffect.SetActive(false);
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
        if (col.gameObject.tag == "Defense")
        {
            transform.position = defenseRespawnPoint.transform.position;
        }

        if (col.gameObject.tag == "Coin")
        {
            SfxSound(0);
        }
    }

    public void Die()
    {
        animator.SetBool("isDie", true);
        isDie = true;
    }

    public void Revive()
    {
        animator.SetBool("isDie", false);
        isDie = false;
        animator.SetBool("isMove", true);
    }
}
