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
        CheckDistanceToPlayer(); //플레이어와의 거리 체크
        IsGroundCheck(); //bool타입을 이용한 점프 체크
        JumpToWall(); //여기서 점프체크 및 점프 실행

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
    //거리가 10이상일때 idle
    //거리가 10미만 1.3초과일때 walk
    //거리가 1.3 미만일때 attack
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
        //Enemy의 Rotation을 담당
        lookDir = (player.position - transform.position).normalized;
        lookDir.y = 0;
        Quaternion from = transform.rotation;
        Quaternion to = Quaternion.LookRotation(lookDir);
        transform.rotation = Quaternion.Lerp(from, to, turnSpeed * Time.deltaTime);

        //Ground와 지속적인 충돌을 감지 및 움직임에 제한을 두기위함
        dir = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        isForwardToWall = Physics.Raycast(dir, transform.forward, 1.0f, LayerMask.GetMask("Ground"));
        if (isForwardToWall)
            speed = 0;
        else
            speed = 3;
        //Enemy의 이동을 담당
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

    //점프 가능 여부를 확인하기위한 메소드
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

    //만약 벽 앞까지 온다면 점프
    void JumpToWall()
    {
        //벡터값 세팅
        dir = new Vector3(transform.position.x, transform.position.y - 0.3f, transform.position.z);
        //에너미가 전방 벽과 가까워졌는지 bool 타입으로 체크
        isBorder = Physics.Raycast(dir, transform.forward, 1.0f, LayerMask.GetMask("Ground"));
        //만약 에너미가 벽과 가까워졌다면 (레이캐스트) 점프
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

