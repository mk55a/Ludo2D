using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{
    [SerializeField] private int positionIndex;
    [SerializeField] private TileType tileType;
    [SerializeField] private TileColor tileColor;

    public TileColor GetTileColor()
    {
        return tileColor;
    }
    public TileType GetTileType()
    {
        return tileType;
    }
    public int GetPositionIndex()
    {
        return positionIndex;
    }
}
