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

    //�̵��� ���� ����
    public Transform player;
    public PlayerMove pm;
    public float distanceFromPlayer; //Enemy�� Player�� �Ÿ��� üũ�ϱ� ����
    public float speed; //�̵��Ҷ� ���ǵ�
    public float turnSpeed; //��(�÷��̾� ��������)���ǵ�
    public E_State e_State;

    //�̵����� �� ��(�׶���)üũ�� ���� ������
    public Transform groundCheckTransform; //���ʹ̸� �������� Ground�� üũ�ϱ� ����
    public Vector3 boxSize = new Vector3(0, 1, 0); //�׶��� üũ�� ������ ���� ���Ͱ�
    public LayerMask groundCheckLayerMask; //�׶����� ���̾�
    public float halfsize = 1; //�׶��� üũ�� ������ ���̱� ���� ����
    public float jumpPower; //�����Ҷ��� ��
    public bool isGround; // ���� isGround��� ������ �Ұ���, !isGround��� ������ ����.
    public bool isBorder; //������⿡ Ground�� üũ���ֱ� ����
    public bool isForwardToWall; //��(Ground)üũ

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


    //�÷��̾���� �Ÿ� ���
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
