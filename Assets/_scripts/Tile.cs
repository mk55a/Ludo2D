using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tile : MonoBehaviour
{

    public List<Unit> unitsOnTile; 

    [SerializeField] private int positionIndex;
    [SerializeField] private TileType tileType;
    [SerializeField] private TileColor tileColor;

    [SerializeField] private GameObject gridLayoutContainer;

    private void Awake()
    {
        unitsOnTile = new List<Unit>();
    }

    public void AddUnit(Unit unit)
    {
        unitsOnTile.Add(unit);
        unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
        Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());
    }

    public void RemoveUnit(Unit unit)
    {
        unitsOnTile.Remove(unit);
        Debug.LogWarning(unit.gameObject.name + " removed from " + GetPositionIndex());
    }

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
