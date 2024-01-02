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
    private float range = 0.5f;  // ������ ������ ������ �ִ� �Ÿ�

    [SerializeField]
    private LayerMask layerMask;  // Ư�� ���̾ ���� ������Ʈ�� ���ؼ��� ������ �� �־�� �Ѵ�.

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

    private void Floating() //���ִ°�
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, range, layerMask))
        {
            rb.AddForce(transform.up * jump, ForceMode.Impulse);
        }
    }
    private void CheckDistanc() // �����Ÿ� ���� ���
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