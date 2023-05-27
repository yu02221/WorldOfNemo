using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public Item item;
    public List<Material> materials = new List<Material>();
    public Dictionary<string, Material> matDictionary = new Dictionary<string, Material>();
    public string matName;

    public int count;
    public Inventory hotInven;
    public Transform player;
    public PlayerMove pm;

    public float jump;

    public Rigidbody rb;

    [SerializeField]
    private float range = 0.5f;  // 아이템 습득이 가능한 최대 거리

    [SerializeField]
    private LayerMask layerMask;  // 특정 레이어를 가진 오브젝트에 대해서만 습득할 수 있어야 한다.

    float dis;
    float itemSpeed = 10f;

    public AudioClip getItemSound;

    private void Start()
    {
        foreach (var material in materials)
        {
            matDictionary.Add(material.name, material);
        }
        GetComponent<MeshRenderer>().material = matDictionary[matName];
        player = GameObject.Find("Player").transform;
        pm = player.GetComponent<PlayerMove>();
        hotInven = pm.hotInven;
    }

    private void Update()
    {
        Floating();
        CheckDistanc();
    }

    private void Floating() //떠있는거
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, range, layerMask))
        {
            rb.AddForce(transform.up * jump, ForceMode.Impulse);
        }
    }
    private void CheckDistanc() // 일정거리 들어가면 드랍
    {
        dis = Vector3.Distance(player.position, transform.position);
        //float distance = Vector3.Distance(player.position, transform.position);
        if (dis <= 2)
        {
            transform.position = Vector3.MoveTowards(transform.position, player.position, itemSpeed * Time.deltaTime);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.name == "Player")
        {
            pm.audioSource.clip = getItemSound;
            pm.audioSource.Play();
            hotInven.AddItem(item, count);
            Destroy(gameObject);
        }
    }
}