using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    //�����Ҷ� ����ϴ� ������
    public int crushPower = 2;
    float crushTime;
    Vector3 playerTransSave;
    float readyTime;

    private float burnTime;

    private void Start()
    {
        player = GameObject.Find("Player").transform;
        rb = GetComponent<Rigidbody>();
        pm = player.GetComponent<PlayerMove>();
        e_State = E_State.Idle;
    }

    private void Update()
    {
        CheckDistanceToPlayer(); //�÷��̾���� �Ÿ� üũ
        IsGroundCheck(); //boolŸ���� �̿��� ���� üũ
        JumpToWall(); //���⼭ ����üũ �� ���� ����

        if (pm.playerState == PlayerState.Dead)
            e_State = E_State.Idle;

        switch (e_State)
        {
            case E_State.Idle:
                Idle();
                break;
            case E_State.Walk:
                Walk();
                break;
            case E_State.Ready:
                Ready();
                break;
            case E_State.Crush:
                Crush();
                break;
            case E_State.Damaged:
                Damaged();
                break;
        }

        if (DayAndNight.tState == DayAndNight.TimeState.Day)
        {
            if (hp == maxHp)
            {
                exp = 0;
                dropItem = null;
            }
            burnTime += Time.deltaTime;
            if (burnTime > 2f)
            {
                burnTime = 0;
                HitByPlayer(lookDir, 3);
            }
        }
    }
    //�Ÿ��� 20 �̸� Walk
    //�Ÿ��� 10 �̸� Ready > Crush
    void Idle()
    {
        if (distanceFromPlayer < 20)
            e_State = E_State.Walk;
        else if (distanceFromPlayer > 20)
            e_State = E_State.Idle;
    }

    void Walk()
    {
        anim.SetBool("walk", true);
        //Enemy�� Rotation�� ���
        lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(from, to, turnSpeed * Time.deltaTime);

        //Ground�� �������� �浹�� ���� �� �����ӿ� ������ �α�����
        dir = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        isForwardToWall = Physics.Raycast(dir, transform.forward, 1.0f, LayerMask.GetMask("Ground"));
        if (isForwardToWall)
            speed = 0;
        else
            speed = 2;
        //Enemy�� �̵��� ���
        transform.position = Vector3.MoveTowards
            (transform.position, player.transform.position, speed * Time.deltaTime);

        if(distanceFromPlayer < 7)
        {
            e_State = E_State.Ready;
        }
    }
    
    void Ready()
    {
        //Enemy�� Rotation�� ���
        lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(from, to, turnSpeed * Time.deltaTime);

        anim.SetBool("walk", false);
        readyTime += Time.deltaTime;
        playerTransSave = player.transform.position;
        anim.SetBool("ready", true);
        if (readyTime > 1.3f)
        { 
            Crush();
            print("Ready > Crush");
            anim.SetBool("ready", false);
            readyTime = 0;
        }
        
    }

    void Crush()
    {
        crushTime += Time.deltaTime;

        if (crushTime < 2)
        {
            speed = 5;
            e_State = E_State.Crush;
            transform.position =
                Vector3.MoveTowards(transform.position, playerTransSave, speed * Time.deltaTime);
            anim.SetBool("crush", true);
        }
        if ( crushTime > 2)
        {
            e_State = E_State.Idle;
            print("Crush > Idle");
            anim.SetBool("crush", false);
            crushTime = 0;
            speed = 2;
        }
        if ( distanceFromPlayer > 20 )
        { 
            e_State = E_State.Idle;
            speed = 2;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Player")
        {
            pm.HitByEnemy(collision.transform.position, 1);
            audioSource.clip = attackSound;
            audioSource.Play();
        }
    }

    //���� ���� ���θ� Ȯ���ϱ����� �޼ҵ�
    void IsGroundCheck()
    {
        Collider[] cols = Physics.OverlapBox(groundCheckTransform.position, boxSize * 0.5f,
            groundCheckTransform.rotation,
            groundCheckLayerMask);
        if (cols.Length > 0)
        {
            isGround = true;
        }
        else
        {
            isGround = false;
        }

    }

    //���� �� �ձ��� �´ٸ� ����
    void JumpToWall()
    {
        //���Ͱ� ����
        dir = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        //���ʹ̰� ���� ���� ����������� bool Ÿ������ üũ
        isBorder = Physics.Raycast(dir, transform.forward, 1.8f, LayerMask.GetMask("Ground"));
        //���� ���ʹ̰� ���� ��������ٸ� (����ĳ��Ʈ) ����
        if (isBorder)
            Jump();
        Debug.DrawRay(dir, transform.forward * 1.8f, Color.red);
    }
    void Jump()
    {
        if (isGround == true)
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}
