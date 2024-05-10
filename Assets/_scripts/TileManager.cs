using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [SerializeField] private List<Tile> tiles;
    [SerializeField] public GameObject inMovementParent;
    

    private int maximumIndex =52;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }

    public List<Tile> GetStartTile(Unit unit)
    {
        return tiles.Where(tile => tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()) && tile.GetTileType() == TileType.SAFE).ToList();
    }

    public List<Tile> GetUnitsTileTraversal(Tile currentTile, int roll)
    {
        //add the dice roll value to the tile's position index and return the tile with the resulting postion index and the other tiles in between the current tile and the resulting tile.
        //if current tile position index is one and roll on dice is 5 add, 1+5 which 6 so return a list of tile with tiles of position index, 2,3,4,5,6

        int currentIndex = currentTile.GetPositionIndex();
        int targetIndex = currentIndex + roll;
        if(targetIndex > maximumIndex)
        {
            targetIndex -= maximumIndex;
        }
        Debug.LogError("TARGET INDEX : " + targetIndex);
        List<Tile> traversalTiles = new List<Tile>();
        Debug.LogWarning("CURRENT TILE INDEX : " + currentIndex);

        List<Tile> eligibleTiles = new List<Tile>();
        foreach (Tile t in tiles)
        {
            int tilePosIndex = t.GetPositionIndex();
            
            if (t.GetTileType() != TileType.END && tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
            {
                Debug.Log(tilePosIndex);
                
                eligibleTiles.Add(t);

            }
        }
        if(eligibleTiles.Count > 1)
        {
            eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
        }
        traversalTiles.AddRange(eligibleTiles);
        Debug.Log("Tile to traverse : "+traversalTiles.Count);

        return traversalTiles;
    }

    public List<Tile> GetUnitsTileTraversal(Unit unit)
    {
        List<Tile> traversalTiles = new List<Tile>();
        List<Tile> eligibleTiles = new List<Tile>();

        int diceRoll = DiceHandler.Instance.GetDiceRoll();
        int currentIndex = unit.GetCurrentTile().GetPositionIndex();
        int traversedTilesCount = unit.GetTilesTraversedCount();
        int targetIndex = 0;

        // If the unit's next move has to be on the end tile
        if (traversedTilesCount == 51) 
        {
            GetUnitsEndTraversal(unit);
            currentIndex = 0;
            targetIndex = diceRoll + currentIndex;

            foreach (Tile tile in tiles)
            {
                if (tile.GetTileType() == TileType.END &&
                    tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()) &&
                    tile.GetPositionIndex() > currentIndex &&
                    tile.GetPositionIndex() <= targetIndex)
                {
                    eligibleTiles.Add(tile);
                }
            }

            if (eligibleTiles.Count > 1)
            {
                eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }

            Debug.LogWarning("GETTING TILES : END TILES");
        }

        // If the unit will enter the end tile this turn
        else if (diceRoll + traversedTilesCount > 51)
        {
            int currentTargetIndex = currentIndex + (51 - traversedTilesCount);
            int endDiceRoll = (diceRoll + traversedTilesCount) - 51;

            foreach (Tile tile in tiles)
            {
                if (tile.GetTileType() != TileType.END &&
                    tile.GetPositionIndex() > currentIndex &&
                    tile.GetPositionIndex() <= currentTargetIndex)
                {
                    eligibleTiles.Add(tile);
                    traversedTilesCount++;
                }
            }

            if (eligibleTiles.Count > 1)
            {
                eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }

            List<Tile> endEligibleTiles = new List<Tile>();

            foreach (Tile tile in tiles)
            {
                if (traversedTilesCount == 51 &&
                    tile.GetTileType() == TileType.END &&
                    tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()) &&
                    tile.GetPositionIndex() <= (57 - traversedTilesCount) &&
                    tile.GetPositionIndex() > currentIndex &&
                    tile.GetPositionIndex() <= endDiceRoll)
                {
                    endEligibleTiles.Add(tile);
                }
            }

            if (endEligibleTiles.Count > 1)
            {
                endEligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }

            eligibleTiles.AddRange(endEligibleTiles);

            Debug.LogWarning("GETTING TILES : SWITCHing TO END TILES");
        }

        // If the unit has to traverse normal tiles but reaches index 52
        else if (currentIndex == 52 && traversedTilesCount < 51) 
        {
            currentIndex = 0;
            targetIndex = currentIndex + diceRoll;

            foreach (Tile tile in tiles)
            {
                if (tile.GetTileType() != TileType.END &&
                    tile.GetPositionIndex() > currentIndex &&
                    tile.GetPositionIndex() <= targetIndex)
                {
                    eligibleTiles.Add(tile);
                }
            }

            if (eligibleTiles.Count > 1)
            {
                eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }

            Debug.LogWarning("GETTING TILES : STARTING FROM ONE");
        }

        // If the unit has to traverse normal tiles
        else
        {
            targetIndex = currentIndex + diceRoll;

            Debug.LogError("TARGET INDEX : " + targetIndex);
            Debug.LogWarning("CURRENT TILE INDEX : " + currentIndex);
            Debug.LogWarning("DICE ROLL : " + diceRoll);

            foreach (Tile tile in tiles)
            {
                if (tile.GetTileType() != TileType.END &&
                    tile.GetPositionIndex() > currentIndex &&
                    tile.GetPositionIndex() <= targetIndex)
                {
                    eligibleTiles.Add(tile);
                }
            }

            if (eligibleTiles.Count > 1)
            {
                eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }

            Debug.LogWarning("GETTING TILES : NORMAL TRAVERSAL");
        }

        traversalTiles.AddRange(eligibleTiles);
        Debug.Log("Tile to traverse : " + traversalTiles.Count);

        return traversalTiles;
    }

    public List<Tile> GetUnitsEndTraversal(Unit unit)
    {
      
        List<Tile> traversalTiles = new List<Tile>();
        List<Tile> eligibleTiles = new List<Tile>();

        int diceRoll = DiceHandler.Instance.GetDiceRoll();
        int currentIndex = unit.GetCurrentTile().GetPositionIndex();
        int targetIndex = currentIndex + diceRoll;
        int traversedTilesCount = unit.GetTilesTraversedCount();
        foreach (Tile tile in tiles)
        {
            int tilePosIndex = tile.GetPositionIndex();
            if (tile.GetTileType() == TileType.END && tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()) && tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
            {
                Debug.Log(tilePosIndex);
                eligibleTiles.Add(tile);
            }
            //Sorting the tiles. 
            if (eligibleTiles.Count > 1)
            {
                eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
            }
        }

        traversalTiles.AddRange(eligibleTiles);
        Debug.Log("Tile to traverse : " + traversalTiles.Count);
        return traversalTiles;
    }

    //Can be in any script HelperClass.cs
    public static TileColor ConvertColorToTileColor(Color color)
    {
        switch (color)
        {
            case Color.RED:
                return TileColor.RED;
            case Color.BLUE:
                return TileColor.BLUE;
            case Color.YELLOW:
                return TileColor.YELLOW;
            case Color.GREEN:
                return TileColor.GREEN;
            default:
                throw new ArgumentException("Unknown color");
        }
    }

}
