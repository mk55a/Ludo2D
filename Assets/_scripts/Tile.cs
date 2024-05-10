using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{
    [Header("Tile Properties")]
    [SerializeField] private int positionIndex;
    [SerializeField] private TileType tileType;
    [SerializeField] private TileColor tileColor;
    [SerializeField] private GameObject gridLayoutContainer;
    [SerializeField] private Image tileImage;
    [SerializeField] private Image safeImage;

    [Header("Common Tile Colors")]
    [SerializeField] private Color32 blue;
    [SerializeField] private Color32 red;
    [SerializeField] private Color32 yellow;
    [SerializeField] private Color32 green;
    [SerializeField] private Color32 white;

    public List<Unit> unitsOnTile;

    private void Awake()
    {
        unitsOnTile = new List<Unit>();
        SetTileProperties();
    }

    private void SetTileProperties()
    {
        switch (tileColor)
        {
            case TileColor.BLUE:
                tileImage.color = blue;
                break;
            case TileColor.RED:
                tileImage.color = red;
                break;
            case TileColor.GREEN:
                tileImage.color = green;
                break;
            case TileColor.YELLOW:
                tileImage.color = yellow;
                break;
            case TileColor.WHITE:
                tileImage.color = white;
                break;
        }

        if (tileType == TileType.SAFE)
        {
            safeImage.gameObject.SetActive(true);
        }
    }

    public void AddUnit(Unit unit)
    {
        if (unitsOnTile.Count == 1 && unitsOnTile[0].GetUnitColor() != unit.GetUnitColor())
        {
            unitsOnTile[0].TraverseBackToHome();
        }

        unitsOnTile.Add(unit);
        unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
        Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());

        if (GetTileColor() == Helper.ConvertColorToTileColor(unit.GetUnitColor()) && GetTileType() == TileType.END && GetPositionIndex() == 6)
        {
            unit.UnitFinished();
        }

        if (GetTileType() == TileType.END && unit.GetUnitState() != UnitState.ONEND)
        {
            unit.UnitIsOnEnd();
        }
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
