using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Tile : MonoBehaviour
{

    public List<Unit> unitsOnTile; 

    [SerializeField] private int positionIndex;
    [SerializeField] private TileType tileType;
    [SerializeField] private TileColor tileColor;

    [SerializeField] private GameObject gridLayoutContainer;





    [Tooltip("CommonProperties")]
    [SerializeField]
    private Color32 blue;
    [SerializeField]
    private Color32 red;
    [SerializeField]
    private Color32 yellow;
    [SerializeField]
    private Color32 green;
    [SerializeField]
    private Color32 white;


    [SerializeField]
    private Image tileImage;

    [SerializeField]
    private Image safeImage;

    private void Awake()
    {
        SetTileProperties();
        unitsOnTile = new List<Unit>();
        
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
        if (unitsOnTile.Count == 1 )
        {
            if(unitsOnTile[0].GetUnitColor() != unit.GetUnitColor())
            {
                unitsOnTile[0].TraverseBackToHome();
                unitsOnTile.Add(unit);
                unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
                Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());
            }
            else
            {
                unitsOnTile.Add(unit);
                unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
                Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());
            }
        }
        else if(unitsOnTile.Count >= 2 )
        {
            if (unitsOnTile[0].GetUnitColor() != unit.GetUnitColor())
            {
                unit.TraverseBackToHome();
            }
            else
            {
                unitsOnTile.Add(unit);
                unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
                Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());
            }
        }
        else
        {
            unitsOnTile.Add(unit);
            unit.gameObject.transform.SetParent(gridLayoutContainer.transform);
            Debug.LogWarning(unit.gameObject.name + " added to " + GetPositionIndex());
        }
        

        if(GetTileColor() == TileManager.ConvertColorToTileColor(unit.GetUnitColor()) && GetTileType()==TileType.END && GetPositionIndex()==6)
        {
            unit.UnitFinished();
        }
        if(GetTileType() == TileType.END && unit.GetUnitState() != UnitState.ONEND)
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
