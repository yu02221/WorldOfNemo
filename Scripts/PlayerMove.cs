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
    //���� �÷��̾��� ����
    public PlayerState playerState;
    //�÷��̾� �ɷ�ġ
    PlayerStatus ps;

    //AnimationClip�� �̿��� ���¿� ���� ���� ��������
    //�������
    public Animator playerAnim;
    //�������

    //�÷��̾� ������ ���� ���� ����
    public float turnSpeed;
    public float moveSpeed;
    public float runSpeed = 8;
    public float walkSpeed = 4;
    Rigidbody rb;
    //�� ������� �Ÿ� ����
    bool isBorder;
    //��

    //�÷��̾� ���� ���� ���� ����
    public Transform groundCheckTransform;
    public Vector3 boxSize = new Vector3(0f, 1f,01f);
    public float halfsize = 1;
    public LayerMask groundCheckLayerMask;
    public float jumpPower;
    bool isGround;
    //��

    TerrainChunk tc;

    Vector3 dir;

    public Text velY;

    // �÷��̾� ���� ���� ����
    public LayerMask enemyLayer;
    public float maxDist;       // ���� ���� �Ÿ�
    Enemy enemy;
    public float attackCool;

    public GameObject deadWindow;

    // �÷��̾� ����
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
            //���콺 �����ӿ� ���� ���� �޴´�
            float yRotateSize = Input.GetAxis("Mouse X") * turnSpeed * Time.deltaTime;
            //���� ���� �����Ѵ�
            float yRotate = transform.eulerAngles.y + yRotateSize;
            //���� ���콺 �������� ���ٸ� ���Ͱ��� ���� ���� 0���� ������Ų��.
            if (yRotateSize == 0)
                rb.angularVelocity = Vector3.zero;
            //�÷��̾��� ȸ���������� ������ ��������ش�
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
        //playerState������ ���� �Է°��� ���� State������ �����Ѵ�
        //�÷��̾ ���� ���⿡ ���� �������� �Է°��� ������ش�.
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

    //�÷��̾��� �ϴ�(�߹ٴ�)�ʿ� üũ�ڽ� ������Ʈ�� ��ġ���� Ground�� ���̾� �����̵� ������Ʈ�� �ݶ��̴��� ��ġ�� �Ǹ� ������ �����ϰԲ� boolüũ�� ���ش�
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
