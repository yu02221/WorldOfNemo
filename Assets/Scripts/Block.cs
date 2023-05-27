using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 블럭의 종류별로 각 면에 맞는 타일 정보를 저장 및
/// 블록 타입을 정의
/// </summary>
public class Block
{
    public Tile top, front, side, bottom;   // 블럭 렌더링을 위한 타일

    public TilePos topPos, frontPos, sidePos, bottomPos;    // 타일 위치

    // 모든 면이 동일한 블럭
    public Block(Tile tile)
    {
        top = front = side = bottom = tile;
        GetPositions();
    }

    // 면마다 타일이 다른 블록
    public Block(Tile top, Tile front, Tile side, Tile bottom)
    {
        this.top = top;
        this.front = front;
        this.side = side;
        this.bottom = bottom;
        GetPositions();
    }

    void GetPositions()
    {
        topPos = TilePos.tiles[top];
        frontPos = TilePos.tiles[front];
        sidePos = TilePos.tiles[side];
        bottomPos = TilePos.tiles[bottom];
    }

    // 블록타입에 알맞은 타일 매칭
    public static Dictionary<BlockType, Block> blocks = new Dictionary<BlockType, Block>(){
        {BlockType.Grass, new Block(Tile.Grass, Tile.GrassSide, Tile.GrassSide, Tile.Dirt)},
        {BlockType.Dirt, new Block(Tile.Dirt)},
        {BlockType.Stone, new Block(Tile.Stone)},
        {BlockType.CobbleStone, new Block(Tile.CobbleStone)},
        {BlockType.OakLog, new Block(Tile.OakLogTop, Tile.OakLogSide, Tile.OakLogSide, Tile.OakLogTop)},
        {BlockType.Leaves, new Block(Tile.Leaves)},
        {BlockType.OakPlanks, new Block(Tile.OakPlanks)},
        {BlockType.Furnace, new Block(Tile.FurnaceTop, Tile.FurnaceFront, Tile.FurnaceSide, Tile.FurnaceTop)},
        {BlockType.Iron, new Block(Tile.Iron)},
        {BlockType.Gold, new Block(Tile.Gold)},
        {BlockType.Diamond, new Block(Tile.Diamond)},
        {BlockType.Coal, new Block(Tile.Coal)},
        {BlockType.CraftingTable, new Block(Tile.CraftingTableTop, Tile.CraftingTableFront, Tile.CraftingTableSide, Tile.OakPlanks)},
        {BlockType.IronBlock, new Block(Tile.IronBlock)},
        {BlockType.GoldBlock, new Block(Tile.GoldBlock)},
        {BlockType.DiamondBlock, new Block(Tile.DiamondBlock)},
        {BlockType.CoalBlock, new Block(Tile.CoalBlock)},
        {BlockType.Bedrock, new Block(Tile.Bedrock)},
    };
}
// 구현된 블럭타입 목록
public enum BlockType 
{
    Air, 
    Dirt, 
    Grass, 
    Stone, 
    CobbleStone, 
    OakLog, 
    Leaves, 
    OakPlanks,
    Furnace,
    Iron,
    Gold,
    Diamond,
    Coal,
    CraftingTable,
    IronBlock,
    GoldBlock,
    DiamondBlock,
    CoalBlock,
    Bedrock,
}