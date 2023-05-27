using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    public enum E_State
    {
        Idle,
        Walk,
        Ready,
        Crush,
        Attack,
        Damaged,
        Death,
    }

    //이동을 위한 변수
    public Transform player;
    public PlayerMove pm;
    public float distanceFromPlayer; //Enemy와 Player의 거리를 체크하기 위함
    public float speed; //이동할때 스피드
    public float turnSpeed; //턴(플레이어 방향으로)스피드
    public E_State e_State;

    //이동관련 중 벽(그라운드)체크를 위한 변수들
    public Transform groundCheckTransform; //에너미를 기준으로 Ground를 체크하기 위함
    public Vector3 boxSize = new Vector3(0, 1, 0); //그라운드 체크의 범위를 위한 벡터값
    public LayerMask groundCheckLayerMask; //그라운드의 레이어
    public float halfsize = 1; //그라운드 체크의 범위를 줄이기 위한 변수
    public float jumpPower; //점프할때의 힘
    public bool isGround; // 만약 isGround라면 점프가 불가능, !isGround라면 점프가 가능.
    public bool isBorder; //정면방향에 Ground를 체크해주기 위함
    public bool isForwardToWall; //벽(Ground)체크

    public Vector3 dir;
    public Vector3 lookDir;

    public Rigidbody rb;
    public int maxHp;
    public int hp;

    public Animator anim;

    public float stateTime;

    public AudioSource audioSource;
    public AudioClip hitSound;
    public AudioClip attackSound;
    public AudioClip deadSound;

    public Item dropItem;
    public int exp;


    //플레이어까지 거리 계산
    public void CheckDistanceToPlayer()
    {
        distanceFromPlayer = Vector3.Distance(transform.position, player.transform.position);
        if (distanceFromPlayer > 128)
            Destroy(gameObject);
    }

    public void HitByPlayer(Vector3 playerPosition, int damage)
    {
        if (e_State != E_State.Death)
        {
            Vector3 reactVec = transform.position - playerPosition;
            reactVec = reactVec.normalized;
            reactVec += Vector3.up;
            rb.AddForce(reactVec * 5, ForceMode.Impulse);
            hp -= damage;
            e_State = E_State.Damaged;
            anim.SetTrigger("damaged");

            audioSource.clip = hitSound;
            audioSource.Play();

            if (hp <= 0)
            {
                Death();
                e_State = E_State.Death;
            }
        }
    }

    public void Damaged()
    {
        stateTime += Time.deltaTime;
        if (hp > 0 && stateTime > 1)
        {
            e_State = E_State.Idle;
            stateTime = 0;
        }
    }


    public void Death()
    {
        audioSource.clip = deadSound;
        audioSource.Play();
        anim.SetBool("death", true);
        e_State = E_State.Death;
        player.GetComponent<PlayerStatus>().GetExp(exp);
        if (dropItem != null)
            pm.hotInven.AddItem(dropItem, 1);
        Destroy(gameObject, 3f);
    }
}
