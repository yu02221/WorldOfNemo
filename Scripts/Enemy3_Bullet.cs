using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy3_Bullet : MonoBehaviour
{
    PlayerMove pm;
    public Transform player;
    public float speed;
    public int power;

    Vector3 playerTransSave;
    void Start()
    {
        player = GameObject.Find("Player").transform;
        pm = player.GetComponent<PlayerMove>();
        Destroy(gameObject, 5f);
        playerTransSave = player.transform.position;
    }

    void Update()
    {
        //transform.Translate(Vector3.forward * speed * Time.deltaTime); 앞으로만 나가서 별로..
        transform.position = Vector3.MoveTowards
            (transform.position, playerTransSave, speed * Time.deltaTime );
        if (transform.position == playerTransSave)
            Destroy(gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.collider.name == "Player")
        {
            pm.HitByEnemy(collision.transform.position, power);
            Destroy(gameObject);
        }

        if (collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject);
        }
    }

}
