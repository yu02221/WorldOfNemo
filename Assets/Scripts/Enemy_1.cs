using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_1 : Enemy
{
    public int attackPower;
    public float attackDelay;
    public float waitAttackTrig;

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
            case E_State.Damaged:
                Damaged();
                break;
            case E_State.Attack:
                Attack();
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
    //�Ÿ��� 10�̻��϶� idle
    //�Ÿ��� 10�̸� 1.3�ʰ��϶� walk
    //�Ÿ��� 1.3 �̸��϶� attack
    void Idle()
    {
        if (distanceFromPlayer > 10 && e_State != E_State.Damaged)
            e_State = E_State.Idle;
        anim.SetBool("walk", false);

        if (distanceFromPlayer < 10 && distanceFromPlayer > 1.3f && e_State != E_State.Damaged)
            e_State = E_State.Walk;
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
            speed = 3;
        //Enemy�� �̵��� ���
        transform.position = Vector3.MoveTowards(transform.position, player.transform.position, speed * Time.deltaTime);

        if (distanceFromPlayer > 10)
            e_State = E_State.Idle;
        else if (distanceFromPlayer < 1.3f)
        { 
            e_State = E_State.Attack;
            anim.SetBool("walk", false);
        }
    }
    void Attack()
    {
        attackDelay += Time.deltaTime;
        waitAttackTrig += Time.deltaTime;
        if (waitAttackTrig > 1)
        {
            anim.SetTrigger("attack");
            audioSource.clip = attackSound;
            audioSource.Play();
            waitAttackTrig = 0;
        }
        if (attackDelay > 2.0f)
        {
            pm.HitByEnemy(transform.position, attackPower);
            attackDelay = 0;
        }
        
        if (distanceFromPlayer > 1.3f)
        {
            e_State = E_State.Idle;
            waitAttackTrig = 0;
            attackDelay = 0;
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
        dir = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        //���ʹ̰� ���� ���� ����������� bool Ÿ������ üũ
        isBorder = Physics.Raycast(dir, transform.forward, 1.0f, LayerMask.GetMask("Ground"));
        //���� ���ʹ̰� ���� ��������ٸ� (����ĳ��Ʈ) ����
        if (isBorder && isGround)
            Jump();
        Debug.DrawRay(dir, transform.forward * 1.0f, Color.red);
    }
    void Jump()
    {
        if (isGround == true)
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
    }
}

