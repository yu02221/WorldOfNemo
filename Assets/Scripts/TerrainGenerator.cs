using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// �÷��̾� ��ġ�� ���� ûũ�� ���� �� ��Ȱ��ȭ
/// </summary>
public class TerrainGenerator : MonoBehaviour
{
    public Transform player;    // �÷��̾� ��ġ
    public List<GameObject> enemyPool = new List<GameObject>(); // ������ų ���ʹ� ���
    private List<Vector3> spawnPos = new List<Vector3>();   // ���ʹ� ���� ��ġ ���
    private bool isSpawned = false;     // �̹� ���Ͱ� ������ ûũ���� ����

    public GameObject terrainChunk;

    public static Dictionary<ChunkPos, TerrainChunk> buildedChunks;
    private int curChunkPosX;
    private int curChunkPosZ;

    public int cWidth;
    public int cHeight;
    public int cDistance;

    public float terrainDetail;
    public float terrainHeight;

    int seed;
    
    private List<ChunkPos> toGenerate = new List<ChunkPos>();

    bool resetSpawned;


    private void Start()
    {
        seed = Random.Range(100000, 999999);
        // �÷��̾��� y�� ��ġ�� ��ǥ������ ����
        player = GameObject.Find("Player").transform;
        int playerY = (int)(Mathf.PerlinNoise(
            (player.position.x / 2 + seed) / terrainDetail, 
            (player.position.z / 2 + seed) / terrainDetail)
            * terrainHeight) + 19;
        player.position = new Vector3(player.position.x, playerY, player.position.z);
        PlayerStatus ps = player.GetComponent<PlayerStatus>();
        ps.spawnPoint = player.position;
        buildedChunks = new Dictionary<ChunkPos, TerrainChunk>();

        //���� ûũ ��ġ ����
        curChunkPosX = Mathf.FloorToInt(player.position.x / 16);
        curChunkPosZ = Mathf.FloorToInt(player.position.z / 16);

        cWidth = TerrainChunk.chunkWidth;
        cHeight = TerrainChunk.chunkHeight;
        // ���۽� �÷��̾� ������ ûũ ��� ����
        LoadChunk(true);
    }

    private void Update()
    {
        int curPosX = Mathf.FloorToInt(player.position.x / 16);
        int curPosZ = Mathf.FloorToInt(player.position.z / 16);
        // ���� �� �ִ� ûũ�� �ٲ���� �� ��ġ�� ���� �ֺ� ûũ ���� �� ��Ȱ��ȭ
        if (curChunkPosX != curPosX || curChunkPosZ != curPosZ)
        {
            // ���� ûũ��ġ ����
            curChunkPosX = curPosX;
            curChunkPosZ = curPosZ;
            // ûũ ����(�̹� ������ ûũ�� Ȱ��ȭ)
            LoadChunk();
            // ûũ ��Ȱ��ȭ
            UnloadChunk();
        }

        if (DayAndNight.tState == DayAndNight.TimeState.Night)
        {
            if (resetSpawned)
                resetSpawned = false;
            foreach (var cPos in buildedChunks.Keys)
            {
                if (buildedChunks[cPos].enemySpawnd || !buildedChunks[cPos].gameObject.activeSelf)
                    continue;
                int chunkDistX = Mathf.Abs(cPos.x - curChunkPosX);
                int chunkDistZ = Mathf.Abs(cPos.z - curChunkPosZ);
                if ((chunkDistX >= 2 || chunkDistZ >= 2) &&
                    chunkDistX <= 8 && chunkDistZ <= 8)
                {
                    SpawnEnemy(buildedChunks[cPos].blocks, cPos.x, cPos.z);
                    buildedChunks[cPos].enemySpawnd = true;
                }
            }
            if (!isSpawned)
                StartCoroutine(DelaySpawnEnemy());
        }

        if (!resetSpawned && DayAndNight.tState == DayAndNight.TimeState.Day)
        {
            foreach (var cPos in buildedChunks.Keys)
            {
                if (buildedChunks[cPos].enemySpawnd)
                    buildedChunks[cPos].enemySpawnd = false;
            }
            resetSpawned = true;
        }
    }
    // ������� ûũ �ε�(���� �Ǵ� Ȱ��ȭ)
    private void LoadChunk(bool instant = false)
    {
        for (int i = curChunkPosX - cDistance; i <= curChunkPosX + cDistance; i++)
        {
            for (int j = curChunkPosZ - cDistance; j <= curChunkPosZ + cDistance; j++)
            {
                if (instant)
                    BuildChunk(i, j);
                else
                    toGenerate.Add(new ChunkPos(i, j));
            }
        }
        // ûũ�� �ٷ� �����ϸ� ���� ������ �߻��Ͽ� ������ ûũ ����� ���� �� ���������� ����
        StartCoroutine(DelayBuildChunks());
    }

    // �־��� ûũ ��Ȱ��ȭ
    private void UnloadChunk()
    {
        List<ChunkPos> toUnload = new List<ChunkPos>();
        foreach (var chunk in buildedChunks)
        {
            ChunkPos cPos = chunk.Key;
            if (Mathf.Abs(curChunkPosX - cPos.x) > (cDistance + 5) ||
                Mathf.Abs(curChunkPosZ - cPos.z) > (cDistance + 5))
            {
                toUnload.Add(chunk.Key);
            }
        }

        while (toUnload.Count > 0)
        {
            buildedChunks[toUnload[0]].gameObject.SetActive(false);
            toUnload.RemoveAt(0);
        }
    }

    // ûũ�� ���� �Ǵ� Ȱ��ȭ
    private void BuildChunk(int xPos, int zPos)
    {
        TerrainChunk chunk;
        ChunkPos curChunk = new ChunkPos(xPos, zPos);
        if (buildedChunks.ContainsKey(curChunk))
        {   // �̹� ������ ûũ�� Ȱ��ȭ
            chunk = buildedChunks[curChunk];
            if (!chunk.gameObject.activeSelf)
            {
                chunk.gameObject.SetActive(true);
            }
            else
                return;
        }
        else
        {   // ������ �� ���� ûũ�� ���� ����
            GameObject chunkObj = Instantiate(terrainChunk, new Vector3(xPos * 16, 0, zPos * 16), Quaternion.identity);
            chunkObj.transform.parent = GameObject.Find("Terrain").transform;
            chunk = chunkObj.GetComponent<TerrainChunk>();
            buildedChunks.Add(curChunk, chunk);

            for (int x = 0; x < cWidth; x++)
                for (int z = 0; z < cWidth; z++)
                    for (int y = 0; y < cHeight; y++)
                        chunk.blocks[x, y, z] = GetBlockType(xPos * 16 + x - 1, y, zPos * 16 + z - 1);

            // ���� ����
            BuildTrees(chunk.blocks);  
            // ���� ���� ����
            BuildOres(chunk.blocks);

            
        }
        // �޽� �׸���
        chunk.BuildMesh();
    }

    // PerlinNoise �Լ��� Ȱ���� �ڿ������� ���� ����
    private BlockType GetBlockType(int x, int y, int z)
    {
        BlockType bt;

        int grassY = (int)(Mathf.PerlinNoise((x / 2 + seed) / terrainDetail, (z / 2 + seed) / terrainDetail) * terrainHeight) +16;
        int soilRange = Random.Range(3, 5);

        if (y == 0)
            bt = BlockType.Bedrock;
        else if (y > grassY)
            bt = BlockType.Air;
        else if (y == grassY)
            bt = BlockType.Grass;
        else if (y < grassY && y >= grassY - soilRange)
            bt = BlockType.Dirt;
        else
            bt = BlockType.Stone;

        return bt;
    }

    private void BuildTrees(BlockType[,,] blocks)
    {
        int ranX = Random.Range(5, 16);
        int ranZ = Random.Range(5, 16);
        for (int x = ranX; x < cWidth - 2; x += ranX)
        {
            ranX = Random.Range(5, 16);
            for (int z = ranZ; z < cWidth - 2; z += ranZ)
            {
                ranZ = Random.Range(5, 16);
                for (int y = 1; y < cHeight - 5; y++)
                {
                    if (blocks[x, y - 1, z] == BlockType.Grass && blocks[x, y, z] == BlockType.Air)
                    {
                        int height = Random.Range(4, 7);
                        for (int i = 0; i < height -2; i++)
                        {
                            blocks[x, y + i, z] = BlockType.OakLog;
                        }
                        for (int i = height - 2; i < height; i++)
                        {
                            blocks[x, y + i, z] = BlockType.OakLog;
                            for (int j = -2; j <= 2; j++)
                            {
                                for (int k = -2; k <= 2; k++)
                                {
                                    if (j != 0 || k != 0)
                                        blocks[x + j, y + i, z + k] = BlockType.Leaves;
                                }
                            }
                        }
                        for (int i = -1; i <= 1; i++)
                        {
                            for (int j = -1; j <= 1; j++)
                            {
                                blocks[x + i, y + height, z + j] = BlockType.Leaves;
                            }
                        }
                    }
                }
            }
        }
    }

    private void BuildOres(BlockType[,,] blocks)
    {
        BuildDiamond(blocks);
        BuildGold(blocks);
        BuildIron(blocks);
        BuildCoal(blocks);
    }

    private void BuildDiamond(BlockType[,,] blocks)
    {
        int randX = Random.Range(0, 16);
        int randY = Random.Range(1, 16);
        int randZ = Random.Range(0, 16);
        int count = Random.Range(2, 9);
        for (int i = 0; i < 2 && randX + i < cWidth; i++)
        {
            for (int j = 0; j < 2; j++)
            {
                for (int k = 0; k < 2 && randZ + k < cWidth; k++)
                {
                    if (count > 0 && 
                        blocks[randX + i, randY + j, randZ + k] == BlockType.Stone)
                    {
                        blocks[randX + i, randY + j, randZ + k] = BlockType.Diamond;
                        count--;
                    }
                }
            }
        }
    }

    private void BuildGold(BlockType[,,] blocks)
    {
        int randX = Random.Range(0, 16);
        int randZ = Random.Range(0, 16);
        for (int x = randX; x < cWidth; x += randX)
        {
            randX = Random.Range(0, 16);
            for (int z = randZ; z < cWidth; z += randZ)
            {
                randZ = Random.Range(0, 16);
                int randY = Random.Range(1, 20);
                int count = Random.Range(2, 9);
                for (int i = 0; i < 2 && randX + i < cWidth; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2 && randZ + k < cWidth; k++)
                        {
                            if (count > 0 &&
                                blocks[randX + i, randY + j, randZ + k] == BlockType.Stone)
                            {
                                blocks[randX + i, randY + j, randZ + k] = BlockType.Gold;
                                count--;
                            }
                        }
                    }
                }
            }
        }
    }

    private void BuildIron(BlockType[,,] blocks)
    {
        int randX = Random.Range(0, 8);
        int randZ = Random.Range(0, 8);
        for (int x = randX; x < cWidth; x += randX)
        {
            randX = Random.Range(0, 8);
            for (int z = randZ; z < cWidth; z += randZ)
            {
                randZ = Random.Range(0, 8);
                int randY = Random.Range(5, 30);
                int count = Random.Range(2, 9);
                for (int i = 0; i < 2 && randX + i < cWidth; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2 && randZ + k < cWidth; k++)
                        {
                            if (count > 0 &&
                                blocks[randX + i, randY + j, randZ + k] == BlockType.Stone)
                            {
                                blocks[randX + i, randY + j, randZ + k] = BlockType.Iron;
                                count--;
                            }
                        }
                    }
                }
            }
        }
    }

    private void BuildCoal(BlockType[,,] blocks)
    {
        int randX = Random.Range(0, 4);
        int randZ = Random.Range(0, 4);
        for (int x = randX; x < cWidth; x += randX)
        {
            randX = Random.Range(0, 4);
            for (int z = randZ; z < cWidth; z += randZ)
            {
                randZ = Random.Range(0, 4);
                int randY = Random.Range(10, 30);
                int count = Random.Range(5, 9);
                for (int i = 0; i < 2 && randX + i < cWidth; i++)
                {
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < 2 && randZ + k < cWidth; k++)
                        {
                            if (count > 0 &&
                                blocks[randX + i, randY + j, randZ + k] == BlockType.Stone)
                            {
                                blocks[randX + i, randY + j, randZ + k] = BlockType.Coal;
                                count--;
                            }
                        }
                    }
                }
            }
        }
    }

    private void SpawnEnemy(BlockType[,,] blocks, int xPos, int zPos)
    {
        int population = Random.Range(0, 2);
        for (int i = 0; i < population; i++)
        {
            int randX = Random.Range(0, 16);
            int randZ = Random.Range(0, 16);
            for (int y = 1; y < 64; y++)
            {
                if (blocks[randX, y - 1, randZ] != BlockType.Air &&
                    blocks[randX, y - 1, randZ] != BlockType.Leaves &&
                    blocks[randX, y, randZ] == BlockType.Air)
                {
                    spawnPos.Add(new Vector3(xPos * 16 + randX, y, zPos * 16 + randZ));
                }
            }
        }
    }

    IEnumerator DelaySpawnEnemy()
    {
        isSpawned = true;
        while (spawnPos.Count > 0)
        {
            print(spawnPos.Count);
            int index = Random.Range(0, enemyPool.Count);
            GameObject enemy = Instantiate(enemyPool[index]);
            enemy.transform.position = spawnPos[0];
            spawnPos.RemoveAt(0);

            yield return null;
        }
        isSpawned = false;
    }

    IEnumerator DelayBuildChunks()
    {
        while (toGenerate.Count > 0)
        {
            BuildChunk(toGenerate[0].x, toGenerate[0].z);
            toGenerate.RemoveAt(0);

            yield return new WaitForSeconds(.01f);
        }
    }
}

public struct ChunkPos
{
    public int x, z;
    public ChunkPos(int x, int z)
    {
        this.x = x;
        this.z = z;
    }
}
