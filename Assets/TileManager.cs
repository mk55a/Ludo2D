using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileManager : MonoBehaviour
{
    public static TileManager Instance { get; private set; }

    [SerializeField] private List<Tile> tiles;
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
    public List<Tile> GetUnitsTileTraversal(Tile tile, int roll)
    {
        //add the dice roll value to the tile's position index and return the tile with the resulting postion index and the other tiles in between the current tile and the resulting tile.
        //if current tile position index is one and roll on dice is 5 add, 1+5 which 6 so return a list of tile with tiles of position index, 2,3,4,5,6
        int currentIndex = tile.GetPositionIndex();
        int targetIndex = currentIndex + roll;
        Debug.LogError("TARGET INDEX : " + targetIndex);
        List<Tile> traversalTiles = new List<Tile>();
        Debug.LogWarning("CURRENT TILE INDEX : " + currentIndex);
        foreach (Tile t in tiles)
        {
            int tilePosIndex = t.GetPositionIndex();
            
            if (tile.GetTileType() != TileType.END)
            {
                if(tilePosIndex > currentIndex && tilePosIndex <= targetIndex)
                {
                    Debug.Log(tilePosIndex);
                    traversalTiles.Add(tile);
                    
                }
            }
        }
        Debug.Log("Tile to traverse : "+traversalTiles.Count);
        return traversalTiles;
    }
}
