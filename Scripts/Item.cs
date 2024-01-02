using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class Item : ScriptableObject
{
    public enum ItemType
    {
        Block,      //��
        Equipment,  //���
        Ingredient, //���
        Food,
        Other,
    }

    public enum MaterType
    {
        Stone,
        Wood,
        Dirt,
        Other,
    }

    public ItemType itemType;
    public BlockType blockType;
    public MaterType materType;
    public string itemName;
    public Sprite itemImage;
    public int maxStorageCount;
    public float burningTime;
    public float hardness;
    public float miningSpeed;
    public float loggingSpeed;
    public float diggingSpeed;
    public int power;
    public int foodPoint;
    public AudioClip hitSound;
    public AudioClip breakSound;
    public AudioClip placeSound;
    public AudioClip stepSound;
}
