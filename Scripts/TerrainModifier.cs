using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

/// <summary>
/// 블럭 채굴(획득) 및 설치
/// </summary>
public class TerrainModifier : MonoBehaviour
{
    public Transform player;
    private PlayerStatus ps;
    private PlayerMove pm;
    public Animator handAnim;

    public LayerMask groundLayer;

    public float maxDist = 4;

    private float durability;
    private float curDurability;
    private string materType;
    public float miningSpeed;
    public float loggingSpeed;
    public float diggingSpeed;

    int curBlockX = TerrainChunk.chunkWidth;
    int curBlockY = TerrainChunk.chunkHeight;
    int curBlockZ = TerrainChunk.chunkWidth;

    int bix;
    int biy;
    int biz;

    TerrainChunk tc;

    public Inventory hotInven;      // 화면 하단의 단축 인벤토리
    public Inventory hotInven_w;    // 인벤토리 윈도우 안의 단축 인벤토리
    public ItemSet itemSet;

    private int curSlot = 0;

    public GameObject inventoryWindow;
    public GameObject craftringTableWindow; // 제작대
    public GameObject furnaceWindow;        // 화로

    public List<GameObject> handedItems;
    private Item curItem;

    public GameObject selectedBlock;
    public GameObject breakingBlock;
    public List<Material> bBlockMaterials;

    public AudioSource hitBlockSound;
    public AudioSource blockSound;

    public GameObject dropItem;

    private void Start()
    {
        ps = player.GetComponent<PlayerStatus>();
        pm = player.GetComponent<PlayerMove>();
        SetCurSlot(curSlot);
        SetHandedItem();
    }

    private void Update()
    {
        MouseInput();
        HotkeyInput();
        ScrollInput();

        CopyHotInven();
        if (curItem != hotInven_w.slots[curSlot].item)
        {
            SetHandedItem();
        }

        GetTargetBlock(1);
    }
    // 마우스 클릭으로 블록 채굴 및 설치
    private void MouseInput()
    {
        bool leftClick = Input.GetMouseButton(0);
        bool rightClick = Input.GetMouseButtonDown(1);
        if (!EventSystem.current.IsPointerOverGameObject())
        {
            if (leftClick)
            {
                handAnim.SetBool("leftClick", true);
                // 거리 안에 블럭이 있으면 채광, 아니면 공격
                if (GetTargetBlock(1))
                    MiningBlock();
                else
                    player.GetComponent<PlayerMove>().Attack();
            }
            else
            {
                handAnim.SetBool("leftClick", false);
                breakingBlock.SetActive(false);
                curDurability = durability;
            }

            if (rightClick)
            {
                // 제작대 우클릭시
                if (GetTargetBlock(1) && tc.blocks[bix, biy, biz] == BlockType.CraftingTable)
                {
                    craftringTableWindow.SetActive(true);
                    inventoryWindow.SetActive(true);
                    pm.playerState = PlayerState.OpenInventory;
                    Cursor.visible = true;
                }
                // 화로 우클릭시
                else if (GetTargetBlock(1) && tc.blocks[bix, biy, biz] == BlockType.Furnace)
                {
                    furnaceWindow.SetActive(true);
                    inventoryWindow.SetActive(true);
                    pm.playerState = PlayerState.OpenInventory;
                    Cursor.visible = true;
                }
                else if (hotInven_w.slots[curSlot].item != null)
                {
                    // 거리 내에 블럭 설치가 가능하면 블럭 설치, 아니면 음식인 경우 섭취
                    if (hotInven_w.slots[curSlot].item.itemType == Item.ItemType.Block && GetTargetBlock(-1))
                        PlacingBlock();
                    else if (hotInven_w.slots[curSlot].item.itemType == Item.ItemType.Food)
                        EatFood();
                }
            }
        }
    }
    // 숫자버튼으로 단축 인벤의 슬롯 변경
    private void HotkeyInput()
    {
        if (Input.GetKeyDown(KeyCode.Alpha1))
            SetCurSlot(0);
        else if (Input.GetKeyDown(KeyCode.Alpha2))
            SetCurSlot(1);
        else if (Input.GetKeyDown(KeyCode.Alpha3))
            SetCurSlot(2);
        else if (Input.GetKeyDown(KeyCode.Alpha4))
            SetCurSlot(3);
        else if (Input.GetKeyDown(KeyCode.Alpha5))
            SetCurSlot(4);
        else if (Input.GetKeyDown(KeyCode.Alpha6))
            SetCurSlot(5);
        else if (Input.GetKeyDown(KeyCode.Alpha7))
            SetCurSlot(6);
        else if (Input.GetKeyDown(KeyCode.Alpha8))
            SetCurSlot(7);
        else if (Input.GetKeyDown(KeyCode.Alpha9))
            SetCurSlot(8);
    }
    // 마우스 스크롤로 단축인벤의 슬롯 변경
    private void ScrollInput()
    {
        float wheelInput = Input.GetAxis("Mouse ScrollWheel");
        if (wheelInput < 0)
        {
            if (curSlot == hotInven.slots.Length - 1)
                SetCurSlot(0);
            else
                SetCurSlot(curSlot + 1);
        }
        else if (wheelInput > 0)
        {
            if (curSlot == 0)
                SetCurSlot(hotInven.slots.Length - 1);
            else
                SetCurSlot(curSlot - 1);
        }
    }
    /// <summary>
    /// 인덱스 번호의 슬롯으로 현제 슬롯을 설정
    /// </summary>
    /// <param name="index">[0,8]의 인덱스</param>
    private void SetCurSlot(int index)
    {
        curSlot = index;
        for (int i = 0; i < hotInven.slots.Length; i++)
        {
            if (i == curSlot)
                hotInven.slots[i].selected.SetActive(true);
            else
                hotInven.slots[i].selected.SetActive(false);
        }
    }
    /// <summary>
    /// 현제 슬롯의 아이템을 손에 든 아이템으로 변경
    /// </summary>
    private void SetHandedItem()
    {
        curItem = hotInven_w.slots[curSlot].item;
        handAnim.SetTrigger("changeItem");
        if (curItem != null)
        {
            ps.weaponPower = curItem.power;
            miningSpeed = curItem.miningSpeed;
            loggingSpeed = curItem.loggingSpeed;
            diggingSpeed = curItem.diggingSpeed;
        }
        if (hotInven_w.slots[curSlot].item == null ||
            hotInven_w.slots[curSlot].item.itemType == Item.ItemType.Food ||
            hotInven_w.slots[curSlot].item.itemType == Item.ItemType.Ingredient ||
            hotInven_w.slots[curSlot].item.itemType == Item.ItemType.Other)
        {
            foreach (var item in handedItems)
            {
                if (item.name == "Hand")
                    item.SetActive(true);
                else
                    item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in handedItems)
            {
                if (item.name == hotInven_w.slots[curSlot].item.itemName)
                    item.SetActive(true);
                else
                    item.SetActive(false);
            }
        }
    }
    // 블록 채굴
    private void MiningBlock()
    {   
        if (bix != curBlockX || biy != curBlockY || biz != curBlockZ)
        {   // 에임이 다른 블럭으로 향하면 블록 내구도 초기화
            curDurability = durability;
            breakingBlock.SetActive(false);
            curBlockX = bix;
            curBlockY = biy;
            curBlockZ = biz;
        }
        else
        {   // 마우스로 클릭하는 동안 내구도 감소
            float speed = 1f;
            switch (materType)
            {
                case "Stone":
                    speed += miningSpeed;
                    break;
                case "Wood":
                    speed += loggingSpeed;
                    break;
                case "Dirt":
                    speed += diggingSpeed;
                    break;
                default:
                    speed = 1f;
                    break;
            }
            
            int index = Mathf.FloorToInt((1 - curDurability / durability) * 6f);
            if (index < 6 && index >= 0)
            {
                if (tc.blocks[bix, biy, biz] != BlockType.Air)
                    hitBlockSound.clip = itemSet.iSet[tc.blocks[bix, biy, biz].ToString()].hitSound;
                breakingBlock.SetActive(true);
                breakingBlock.transform.position = selectedBlock.transform.position;
                breakingBlock.GetComponentInChildren<MeshRenderer>().material = bBlockMaterials[index];
            }
            curDurability -= Time.deltaTime * speed * 0.5f;
        }

        if (curDurability <= 0)
        {   // 내구도가 0이하가 되면 블럭 채굴
            if (tc.blocks[bix, biy, biz] != BlockType.Air && !blockSound.isPlaying)
            {
                blockSound.clip = itemSet.iSet[tc.blocks[bix, biy, biz].ToString()].breakSound;
                blockSound.transform.position = breakingBlock.transform.position;
                blockSound.Play();
            }
            curDurability = durability;
            GetItem(tc.blocks[bix, biy, biz]);
            breakingBlock.SetActive(false);

            tc.blocks[bix, biy, biz] = BlockType.Air;
            tc.BuildMesh();
        }
    }
    
    // 단축 인벤의 선택슬롯에 있는 블럭 설치
    private void PlacingBlock()
    {
        if (ps.standBlockX - ps.standChunkX * 16 != bix ||
            ps.standBlockZ - ps.standChunkZ * 16 != biz ||
            ps.standBlockY != biy && ps.standBlockY + 1 != biy)
        {
            blockSound.clip = hotInven_w.slots[curSlot].item.placeSound;
            blockSound.Play();
            tc.blocks[bix, biy, biz] = hotInven_w.slots[curSlot].item.blockType;

            if (--hotInven_w.slots[curSlot].itemCount == 0)
            {
                hotInven_w.slots[curSlot].item = null;
            }

            hotInven_w.slots[curSlot].SetItemCountText();

            tc.BuildMesh();
        }
    }

    /// <summary>
    /// 블럭을 채굴/설치할 위치를 설정
    /// </summary>
    /// <param name="sign">안쪽 블럭 : -1, 바깥쪽 블럭 : 1</param>
    /// <returns>최대 사거리 이내 블럭이 있는지 여부</returns>
    private bool GetTargetBlock(int sign)
    {
        RaycastHit hitInfo;
        if (Physics.Raycast(transform.position, transform.forward, out hitInfo, maxDist, groundLayer))
        {
            Vector3 targetPos = hitInfo.point + transform.forward * .01f * sign;

            int chunkPosX = Mathf.FloorToInt(targetPos.x / 16f);
            int chunkPosZ = Mathf.FloorToInt(targetPos.z / 16f);

            ChunkPos cp = new ChunkPos(chunkPosX, chunkPosZ);

            tc = TerrainGenerator.buildedChunks[cp];

            //index of the target block
            bix = Mathf.FloorToInt(targetPos.x) - chunkPosX * 16;
            biy = Mathf.FloorToInt(targetPos.y);
            biz = Mathf.FloorToInt(targetPos.z) - chunkPosZ * 16;

            selectedBlock.SetActive(true);
            selectedBlock.transform.position = new Vector3(
                bix + chunkPosX * 16, biy, biz + chunkPosZ * 16);

            if (biy >= TerrainChunk.chunkHeight || biy < 0)
                return false;
            GetBlockDurability(tc.blocks[bix, biy, biz]);
            return true;
        }
        else
        {
            selectedBlock.SetActive(false);
            return false;
        }
    }

    private void GetBlockDurability(BlockType targetBlock)
    {
        switch (targetBlock)
        {
            case BlockType.Air:
                materType = "Other";
                break;
            case BlockType.Leaves:
                materType = "Other";
                durability = 1f;
                break;
            case BlockType.Grass:
                durability = 0.6f;
                materType = "Dirt";
                break;
            case BlockType.Coal:
            case BlockType.Iron:
            case BlockType.Gold:
            case BlockType.Diamond:
                durability = 3f;
                materType = "Stone";
                break;
            default:
                durability = itemSet.iSet[targetBlock.ToString()].hardness;
                materType = itemSet.iSet[targetBlock.ToString()].materType.ToString();
                break;
        }
    }

    public void GetItem(BlockType block)
    {
        Item item;
        int count = 1;
        Vector3 offset = new Vector3(0.5f, 0.5f, 0.5f);
        switch (block)
        {
            case BlockType.Grass:
                item = itemSet.iSet["Dirt"];
                break;
            case BlockType.Stone:
                item = itemSet.iSet["CobbleStone"];
                break;
            case BlockType.Coal:
                item = itemSet.iSet["Coal"];
                ps.GetExp(1);
                break;
            case BlockType.Diamond:
                item = itemSet.iSet["Diamond"];
                ps.GetExp(5);
                break;
            case BlockType.Iron:
                item = itemSet.iSet["RawIron"];
                ps.GetExp(2);
                break;
            case BlockType.Gold:
                item = itemSet.iSet["RawGold"];
                ps.GetExp(3);
                break;
            case BlockType.Leaves:
                if (Random.Range(0, 10) > 7)
                    item = itemSet.iSet["Apple"];
                else
                    item = null;
                break;
            default:
                item = itemSet.iSet[block.ToString()];
                break;
        }
        if (item != null)
        {
            if (item.itemType == Item.ItemType.Block)
            {
                GameObject dItemObj = Instantiate(dropItem);
                dItemObj.transform.position = breakingBlock.transform.position + offset;
                DropItem dItem = dItemObj.GetComponent<DropItem>();
                dItem.item = item;
                dItem.count = count;
                dItem.matName = item.itemName;
            }
            else
            {
                hotInven_w.AddItem(item, count);
            }
        }
    }

    private void CopyHotInven()
    {
        for (int i = 0; i < hotInven_w.slots.Length; i++)
        {
            hotInven.slots[i].item = hotInven_w.slots[i].item;
            hotInven.slots[i].itemCount = hotInven_w.slots[i].itemCount;
            hotInven.slots[i].SetItemCountText();
            hotInven_w.slots[i].SetItemCountText();
        }
    }

    private void EatFood()
    {
        if (ps.hunger >= ps.maxHunger)
            return;
        ps.hunger += hotInven_w.slots[curSlot].item.foodPoint;
        if (ps.hunger > ps.maxHunger)
            ps.hunger = ps.maxHunger;
        ps.SetHunger();

        hotInven_w.slots[curSlot].itemCount--;
        if (hotInven_w.slots[curSlot].itemCount == 0)
            hotInven_w.slots[curSlot].item = null;
        hotInven_w.slots[curSlot].SetItemCountText();
    }
}
