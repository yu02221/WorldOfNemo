using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PlayerState
{
    Idle,
    Walk,
    Run,
    Jump,
    Crouch,
    Attack,
    Damaged,
    Dead,
    OpenInventory,
}
public class PlayerMove : MonoBehaviour
{
    //현재 플레이어의 상태
    public PlayerState playerState;
    //플레이어 능력치
    PlayerStatus ps;

    //AnimationClip을 이용해 상태에 대한 동작 구현예정
    //여기부터
    public Animator playerAnim;
    //여기까지

    //플레이어 움직임 관련 변수 시작
    public float turnSpeed;
    public float moveSpeed;
    public float runSpeed = 8;
    public float walkSpeed = 4;
    Rigidbody rb;
    //블럭 측면과의 거리 측정
    bool isBorder;
    //끝

    //플레이어 점프 관련 변수 시작
    public Transform groundCheckTransform;
    public Vector3 boxSize = new Vector3(0f, 1f,01f);
    public float halfsize = 1;
    public LayerMask groundCheckLayerMask;
    public float jumpPower;
    bool isGround;
    //끝

    TerrainChunk tc;

    Vector3 dir;

    public Text velY;

    // 플레이어 공격 관련 변수
    public LayerMask enemyLayer;
    public float maxDist;       // 공격 가능 거리
    Enemy enemy;
    public float attackCool;

    public GameObject deadWindow;

    // 플레이어 사운드
    public AudioSource audioSource;
    public AudioClip walkSound;
    public AudioClip hitSound;
    public AudioClip deadSound;
    public AudioClip attackSound;

    public Inventory hotInven;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        ps = GetComponent<PlayerStatus>();
    }
    private void Update()
    {
        IsGroundCheck();

        if (playerState != PlayerState.Dead && 
            playerState != PlayerState.Attack && 
            playerState != PlayerState.OpenInventory)
        {
            Jump();
            //마우스 움직임에 대한 값을 받는다
            float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
            //받은 값을 저장한다
            float yRotate = transform.eulerAngles.y + yRotateSize;
            //만약 마우스 움직임이 없다면 벡터값에 대한 힘을 0으로 고정시킨다.
            if (yRotateSize == 0)
                rb.angularVelocity = Vector3.zero;
            //플레이어의 회전값에대한 내용을 적용시켜준다
            transform.eulerAngles = new Vector3(0, yRotate, 0);

            if (attackCool > 0)
                attackCool -= Time.deltaTime;

            if (playerState == PlayerState.Walk)
            {
                audioSource.clip = walkSound;
                if (!audioSource.isPlaying)
                    audioSource.Play();
            }
        }

        if (transform.position.y < 0)
            HitByEnemy(dir, ps.hp);
    }

    private void FixedUpdate()
    {
        if (playerState != PlayerState.Dead && 
            playerState != PlayerState.Attack && 
            playerState != PlayerState.OpenInventory)
            Move();

    }

    void Move()
    {
        //playerState변경을 위해 입력값에 대한 State변경을 진행한다
        //플레이어가 보는 방향에 따른 움직임의 입력값을 출력해준다.
        if (Input.GetKey(KeyCode.W) || Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.S) || Input.GetKey(KeyCode.D))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerState = PlayerState.Run;
                playerAnim.SetBool("run", true);
                moveSpeed = runSpeed;
            }
            else
            {
                playerState = PlayerState.Walk;
                playerAnim.SetBool("walk", true);
                playerAnim.SetBool("run", false);
                moveSpeed = walkSpeed;
            }
            
            Vector3 move = transform.forward * Input.GetAxis("Vertical") + transform.right * Input.GetAxis("Horizontal");
            if (MoveBlockCheck(move))
            {
                transform.position += move * moveSpeed * Time.deltaTime;
            }

            playerState = PlayerState.Walk;
            playerAnim.SetBool("walk", true);
        }
        else if(isGround == true)
        {
            playerState = PlayerState.Idle;
            playerAnim.SetBool("walk", false);
            playerAnim.SetBool("run", false);

        }
    }

    //플레이어의 하단(발바닥)쪽에 체크박스 오브젝트를 위치시켜 Ground로 레이어 지정이된 오브젝트와 콜라이더가 겹치게 되면 점프가 가능하게끔 bool체크를 해준다
    void IsGroundCheck()
    {
        Collider[] cols = Physics.OverlapBox(groundCheckTransform.position, boxSize * 0.1f,
            groundCheckTransform.rotation,
            groundCheckLayerMask);
        if (cols.Length > 0)
        { 
            isGround = true;
        }
        else
        { 
            isGround = false;
            playerState = PlayerState.Jump;
        }
    }
    private void Jump()
    {
        if (playerState != PlayerState.Dead && playerState != PlayerState.Damaged &&
            playerState != PlayerState.Attack && playerState != PlayerState.Crouch &&
            isGround == true)
        {
            if (Input.GetButton("Jump"))
            {
                rb.velocity = new Vector2(rb.velocity.x, jumpPower);
                ps.hungerTime += 0.01f;
                //rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            }
            else return;
        }
    }

    private bool MoveBlockCheck(Vector3 move)
    {
        Vector3 moveBlockPos = transform.position + move.normalized * 0.5f;
        int chunkPosX = Mathf.FloorToInt(moveBlockPos.x / 16f);
        int chunkPosZ = Mathf.FloorToInt(moveBlockPos.z / 16f);

        ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);

        int bix = Mathf.FloorToInt(moveBlockPos.x) - chunkPosX * 16;
        int biy = Mathf.FloorToInt(moveBlockPos.y);
        int biz = Mathf.FloorToInt(moveBlockPos.z) - chunkPosZ * 16;

        if (bix < 0 || bix >= 16 || biy < 0 || biy >= 64 || biz < 0 || biz >= 16)
            return false;

        tc = TerrainGenerator.buildedChunks[cp];
        if (tc.blocks[bix, biy, biz] == BlockType.Air)
            return true;
        else
            return false;
    }

    void Dead()
    {
        audioSource.clip = deadSound;
        audioSource.Play();
        deadWindow.SetActive(true);
        Cursor.visible = true;
        rb.velocity = Vector3.zero;
        //Time.timeScale = 0;
    }

    public void HitByEnemy(Vector3 enemyPosition, int damage)
    {
        Vector3 reactVec = transform.position - enemyPosition;
        reactVec = reactVec.normalized;
        reactVec += Vector3.up;
        rb.AddForce(reactVec * 5, ForceMode.Impulse);
        ps.hp -= damage;
        ps.SetHp();

        audioSource.clip = hitSound;
        audioSource.Play();

        if (ps.hp <= 0)
        {
            playerState = PlayerState.Dead;
            Dead();
        }
    }

    public void Attack()
    {
        RaycastHit hitInfo;
        if (attackCool <= 0 &&
            Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hitInfo, maxDist, enemyLayer))
        {
            audioSource.clip = attackSound;
            audioSource.Play();
            attackCool = 1;
            enemy = hitInfo.transform.GetComponent<Enemy>();
            enemy.HitByPlayer(transform.position, 1 + ps.weaponPower);
        }
    }

    public void Respawn()
    {
        Time.timeScale = 1;
        playerState = PlayerState.Idle;
        deadWindow.SetActive(false);
        hotInven.ResetInventory();
        ps.hp = ps.maxHp;
        ps.hunger = ps.maxHunger;
        ps.exp = 0;
        ps.level = 0;
        ps.SetHp();
        ps.SetHunger();
        transform.position = ps.spawnPoint;
        rb.velocity = Vector3.zero;
    }
}
