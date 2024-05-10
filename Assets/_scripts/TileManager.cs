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
    private bool diceRolled;

    private int maximumIndex =52;
    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }

    private void Start()
    {
        
        //Dice.Instance.OnDiceRolled += HandleDiceRolled;
    }

    public List<Tile> GetBlueStart()
    {
        Debug.LogWarning(tiles.Where(tile => tile.GetTileColor() == TileColor.BLUE && tile.GetTileType() == TileType.SAFE).ToList().Count); //Return the tile which is of BLUE TileColor and SAFE TileType   
        return tiles.Where(tile => tile.GetTileColor() == TileColor.BLUE && tile.GetTileType() == TileType.SAFE).ToList();
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
        //add the dice roll value to the tile's position index and return the tile with the resulting postion index and the other tiles in between the current tile and the resulting tile.
        //if current tile position index is one and roll on dice is 5 add, 1+5 which 6 so return a list of tile with tiles of position index, 2,3,4,5,6

        List<Tile> traversalTiles = new List<Tile>();
        List<Tile> eligibleTiles = new List<Tile>();

        int targetIndex=0;

        int diceRoll = DiceHandler.Instance.GetDiceRoll();

        int currentIndex = unit.GetCurrentTile().GetPositionIndex();

        

        int traversedTilesCount = unit.GetTilesTraversedCount();
        //if units next move has to be in the end tile
        if (traversedTilesCount == 51)
        {
            GetUnitsEndTraversal(unit);
            currentIndex = 0;
            targetIndex = diceRoll + currentIndex;
            foreach(Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                if (tile.GetTileType() == TileType.END && tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()) && tilePosIndex>currentIndex && tilePosIndex<= targetIndex)
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



        //if in this turn the unit will enter the end tile
        else if(diceRoll + traversedTilesCount > 51)
        {
            //The remaining dice. 
            int currentTargetIndex = currentIndex + (51 - traversedTilesCount);
           
            int endDiceRoll = (diceRoll + traversedTilesCount) - 51;
            int currentDiceRoll = diceRoll - endDiceRoll;

            foreach (Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                if (tile.GetTileType() != TileType.END && tilePosIndex > currentIndex && tilePosIndex <= currentTargetIndex)
                {
                    Debug.Log(tilePosIndex);
                    eligibleTiles.Add(tile);
                    traversedTilesCount++;
                }
                //Sorting the tiles. 
                if (eligibleTiles.Count > 1)
                {
                    eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
                }
            }
            List<Tile> endEligibleTiles = new List<Tile>();
            foreach (Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                currentIndex = 0;
                if (traversedTilesCount == 51 && tile.GetTileType() == TileType.END && tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()))
                {
                    if (tilePosIndex <= (57 - traversedTilesCount) && tilePosIndex > currentIndex && tilePosIndex<= endDiceRoll)
                    {
                        endEligibleTiles.Add(tile);
                        
                    }
                }
                if (endEligibleTiles.Count > 1)
                {
                    endEligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
                }
            }

            eligibleTiles.AddRange(endEligibleTiles);
            Debug.LogWarning("GETTING TILES : SWITCHing TO END TILES");
        }


        

        //if the unit has to still traverse on the normal tiles, but reaches tile 52 index
        else if(currentIndex==52 && traversedTilesCount < 51)
        {
            currentIndex = 0;
            targetIndex = currentIndex + diceRoll;

            foreach (Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                if (tile.GetTileType() != TileType.END && tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
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
            Debug.LogWarning("GETTING TILES : STATRTING FROM ONE");
        }




        //if it is not going to switch it's state, and traverse on normal tiles only. 
        else
        {
            targetIndex = currentIndex + diceRoll;
            Debug.LogError("TARGET INDEX : " + targetIndex);
            Debug.LogWarning("CURRENT TILE INDEX : " + currentIndex);
            Debug.LogWarning("DICE ROLL : " + diceRoll);
            foreach (Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                if (tile.GetTileType() != TileType.END && tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
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
            Debug.LogWarning("GETTING TILES : NORMAL TRAVERSAL");
        }
        
        

        traversalTiles.AddRange(eligibleTiles);
        Debug.Log("Tile to traverse : " + traversalTiles.Count);

        return traversalTiles;
    }

    public List<Tile> GetUnitsEndTraversal(Unit unit)
    {
        /*//If its already on END type tiles.
        else if (traversedTilesCount > 51)
        {
            targetIndex = currentIndex + diceRoll;
            foreach (Tile tile in tiles)
            {
                int tilePosIndex = tile.GetPositionIndex();
                currentIndex = 0;
                if (traversedTilesCount == 51 && tile.GetTileType() == TileType.END && tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()))
                {
                    if (tilePosIndex <= (57 - traversedTilesCount) && tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
                    {
                        eligibleTiles.Add(tile);

                    }
                }
                if (eligibleTiles.Count > 1)
                {
                    eligibleTiles.Sort((a, b) => a.GetPositionIndex().CompareTo(b.GetPositionIndex()));
                }
            }
            Debug.LogWarning("GETTING TILES : ALREADY ON END TILES going to end");
        }*/


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
