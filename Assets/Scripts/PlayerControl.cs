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

    public Transform[] audioList;
    int playerMapPosition;

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

        if (Dialogue.instance.dialogueRunning || GameManager.isMiniGame)
        {
            FootStepStop(0);
            animator.SetBool("isMove", false);
        }

        if (GaugeBar.time < 0)
        {
            Die();
        }
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
            if (isGrounded)
            {
                FootStepPlay(0);
            }
            else
            {
                FootStepStop(0);
            }

        }
        else
        {
            animator.SetBool("isMove", false);
            FootStepStop(0);
        }
    }

    private void Jump()
    {
        isGrounded = Physics.Raycast(transform.position, Vector3.down, groundCheckDistance, groundLayer);  // 바닥 레이어 확인

        if (isGrounded && Input.GetButtonDown("Jump"))
        {
            animator.SetBool("isJump", true);
            rb.AddForce(Vector3.up * jumpForce, ForceMode.Impulse);
            SfxPlay(3);
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
            FootStepStop(0);
            animator.SetInteger("slash", 1);
            slash = 1;
            slashEffect.SetActive(true);
            SfxPlay(1);
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
            FootStepStop(0);
            animator.SetBool("isGuard", true);
            isGuard = true;
            guardEffect.SetActive(true);
            SfxPlay(2);
        }
        if (Input.GetKeyUp(KeyCode.Q)) // 버튼 떼기
        {
            animator.SetBool("isGuard", false);
            isGuard = false;
            guardEffect.SetActive(false);
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        // 낙사
        if (col.gameObject.tag == "Fall")
        {
            transform.position = respawnPoint.transform.position;
        }

        // 미니게임 시작
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

        // 오브젝트 획득
        if (col.gameObject.tag == "Coin")
        {
            SfxPlay(0);
        }

        // 배경음악 변경
        if (col.gameObject.tag == "MainHall_BGM")
        {
            BgmPlay(0);
        }
        if (col.gameObject.tag == "GameZone_BGM")
        {
            BgmPlay(1);
        }
        /*if (col.gameObject.tag == "Puzzle_BGM")
        {
            BgmPlay(2);
        }
        if (col.gameObject.tag == "SpeedRun_BGM")
        {
            BgmPlay(3);
        }
        if (col.gameObject.tag == "Defense_BGM")
        {
            BgmPlay(4);
        }*/
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

    #region "AudioManage -----------"
    void BgmPlay(int kind)
    {
        audioList[0].SendMessage("BGMPlay", kind);
    }

    void SfxPlay(int kind)
    {
        audioList[1].SendMessage("AudioPlay", kind);
    }

    void FootStepPlay(int kind)
    {
        audioList[2].SendMessage("AudioPlay", kind);
    }

    void FootStepStop(int kind)
    {
        audioList[2].SendMessage("AudioStop", kind);
    }
    #endregion
}
