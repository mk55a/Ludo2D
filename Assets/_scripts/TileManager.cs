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
    public List<Tile> GetUnitsTileTraversal(Tile currentTile, int roll)
    {
        //add the dice roll value to the tile's position index and return the tile with the resulting postion index and the other tiles in between the current tile and the resulting tile.
        //if current tile position index is one and roll on dice is 5 add, 1+5 which 6 so return a list of tile with tiles of position index, 2,3,4,5,6

         

        int currentIndex = currentTile.GetPositionIndex();
        int targetIndex = currentIndex + roll;
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

                
                /*count++;
                Debug.Log("New Count: " + count);
                if (count >= 51)
                {

                }*/
                eligibleTiles.Add(t);

                //traversalTiles.Add(tile);
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

        int targetIndex;

        int diceRoll = OldDice.Instance.GetRoll();

        int currentIndex = unit.GetCurrentTile().GetPositionIndex();

        

        int traversedTilesCount = unit.GetTilesTraversedCount();

        if(diceRoll + traversedTilesCount > 51)
        {
            //The remaining dice. 
            targetIndex = currentIndex + (51 - traversedTilesCount);

            diceRoll = (diceRoll + traversedTilesCount) - 51;
        }
        else
        {
            targetIndex = currentIndex + diceRoll;
        }
        
        
        
        Debug.LogError("TARGET INDEX : " + targetIndex);
        Debug.LogWarning("CURRENT TILE INDEX : " + currentIndex);
        
        foreach (Tile tile in tiles)
        {
            int tilePosIndex = tile.GetPositionIndex();

            if (traversedTilesCount >= 51)
            {
                while (diceRoll > 0 && diceRoll<= (57-traversedTilesCount))
                {
                    if(tile.GetTileType() == TileType.END && tile.GetTileColor() == ConvertColorToTileColor(unit.GetUnitColor()))
                    {

                        // make sure to the least position index ones to the eligibleTiles.
                    }
                }
            }

            else
            {

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
