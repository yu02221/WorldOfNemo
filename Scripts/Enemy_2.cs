using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_2 : Enemy
{
    //공격할때 사용하는 변수들
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
    //거리가 20 미만 Walk
    //거리가 10 미만 Ready > Crush
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
            speed = 2;
        //Enemy의 이동을 담당
        transform.position = Vector3.MoveTowards
            (transform.position, player.transform.position, speed * Time.deltaTime);

        if(distanceFromPlayer < 7)
        {
            e_State = E_State.Ready;
        }
    }
    
    void Ready()
    {
        //Enemy의 Rotation을 담당
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
        dir = new Vector3(transform.position.x, transform.position.y + 0.2f, transform.position.z);
        //에너미가 전방 벽과 가까워졌는지 bool 타입으로 체크
        isBorder = Physics.Raycast(dir, transform.forward, 1.8f, LayerMask.GetMask("Ground"));
        //만약 에너미가 벽과 가까워졌다면 (레이캐스트) 점프
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
