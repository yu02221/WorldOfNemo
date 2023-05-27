using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerStatus : MonoBehaviour
{
    PlayerMove pm;

    public int standBlockX;
    public int standBlockY;
    public int standBlockZ;

    public int standChunkX;
    public int standChunkZ;

    public LayerMask groundLayer;
    TerrainChunk tc;

    public Text xyz;
    public Text chunkPosTxt;

    public int hp;
    public int maxHp;
    private float recoveryTime;
    public Image[] hearts;
    public Sprite fullHeart;
    public Sprite halfHeart;
    public Sprite emptyHeart;

    public int hunger;
    public int maxHunger;
    public float hungerTime;
    public Image[] hungers;
    public Sprite fullHunger;
    public Sprite halfHunger;
    public Sprite emptyHunger;

    public int exp = 0;
    public int maxExp = 7;
    public int level = 0;
    public Slider expSlider;
    public TMP_Text levelText;

    public int weaponPower;

    public Vector3 spawnPoint;

    public AudioSource expGetSound;

    private void Start()
    {
        pm = GetComponent<PlayerMove>();
        GetStandBlock();
    }

    private void Update()
    {
        GetStandBlock();

        xyz.text = string.Format($"X : {standBlockX}\nY : {standBlockY}\nZ : {standBlockZ}");
        chunkPosTxt.text = string.Format($"ChunkX : {standChunkX}\nChunkZ : {standChunkZ}");

        if (pm.playerState == PlayerState.Run || pm.playerState == PlayerState.Jump)
            hungerTime += 0.1f * Time.deltaTime;
        else
            hungerTime += 0.01f * Time.deltaTime;

        if (hungerTime >= 1f)
        {
            hunger--;
            if (hunger <= 6)
            {
                pm.runSpeed = pm.walkSpeed;
            }
            
            SetHunger();
            hungerTime = 0;
        }
        if (hunger <= 0 && hungerTime > 0.1f)
        {
            hungerTime = 0;
            hunger = 0;
            if (hp >= 2)
            {
                hp--;
                SetHp();
            }
        }

        if (hunger == maxHunger && hp < maxHp)
        {
            if (recoveryTime <= 0)
            {
                recoveryTime = 5f;
                hp++;
                SetHp();
            }
            else
                recoveryTime -= Time.deltaTime;
        }

        if (expSlider.value < exp / (float)maxExp)
            expSlider.value = Mathf.Lerp(expSlider.value, exp / (float)maxExp, 3 * Time.deltaTime);
        else if (expSlider.value > exp / (float)maxExp)
        {
            expSlider.value = Mathf.Lerp(expSlider.value, 1,  3 * Time.deltaTime);
            if (expSlider.value >= 0.99f)
                expSlider.value = 0;
        }
    }

    private void GetStandBlock()
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, -transform.up, out hitInfo, groundLayer))
        {
            Vector3 targetPos = hitInfo.point;

            standChunkX = Mathf.FloorToInt(targetPos.x / 16f);
            standChunkZ = Mathf.FloorToInt(targetPos.z / 16f);

            ChunkPos cp = new ChunkPos(standChunkX, standChunkZ);

            tc = TerrainGenerator.buildedChunks[cp];

            //index of the target block
            standBlockX = Mathf.FloorToInt(targetPos.x);
            standBlockY = Mathf.FloorToInt(transform.position.y - 0.1f);
            standBlockZ = Mathf.FloorToInt(targetPos.z);
        }
    }

    public void SetHp()
    {
        if (hp > maxHp)
            hp = maxHp;

        for (int i = 0; i < hearts.Length; i++)
        {
            if (i < hp / 2)
            {
                hearts[i].sprite = fullHeart;
            }
            else if (i == hp / 2)
            {
                if (hp % 2 == 1)
                    hearts[i].sprite = halfHeart;
                else
                    hearts[i].sprite = emptyHeart;
            }
            else
                hearts[i].sprite = emptyHeart;
        }
    }

    public void SetHunger()
    {
        if (hunger > maxHunger)
            hunger = maxHunger;

        for (int i = 0; i < hungers.Length; i++)
        {
            if (i < hunger / 2)
            {
                hungers[i].sprite = fullHunger;
            }
            else if (i == hunger / 2)
            {
                if (hunger % 2 == 1)
                    hungers[i].sprite = halfHunger;
                else
                    hungers[i].sprite = emptyHunger;
            }
            else
                hungers[i].sprite = emptyHunger;
        }
    }

    public void GetExp(int amount)
    {
        exp += amount;
        expGetSound.Play();
        if (exp >= maxExp)
        {
            exp -= maxExp;
            LevelUp();
        }
    }

    private void LevelUp()
    {
        level++;
        levelText.text = level.ToString();
        maxExp = level * 2 + 7;
    }
}
