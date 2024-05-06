using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private List<UnitHolderBase> unitHolders;
    [SerializeField] private Color homeColor;
    [SerializeField] private GameObject unitPrefab;

    public List<Unit> activeUnits;
    public List<Unit> atHomeUnits;
    public List<Unit> finishedUnits;

    private bool isTurn =true; 
    private void Start()
    {
        activeUnits = new List<Unit>();
        atHomeUnits = new List<Unit>();
        finishedUnits = new List<Unit>();
        OldDice.Instance.OnDiceRolled += HandleDiceRolled;
        //Dice.OnRoll += HandleDiceRolled;
        foreach(UnitHolderBase holders in unitHolders)
        {
            Unit unit =holders.InstantiateUnit(unitPrefab);

            unit.OnStateChanged += HandleUnitStateChanged;
           
        }
    }

    private void HandleUnitStateChanged(Unit unit , UnitState newState)
    {
        switch (newState)
        {
            case UnitState.HOME:
                finishedUnits.Remove(unit);
                activeUnits.Remove(unit);
                atHomeUnits.Add(unit);

                break;

            case UnitState.ONBOARD:
                finishedUnits.Remove(unit);
                atHomeUnits.Remove(unit);
                activeUnits.Add(unit);

                break;

            case UnitState.FINISH:
                atHomeUnits.Remove(unit);
                activeUnits.Remove(unit);
                finishedUnits.Add(unit);

                break;


        }
    }


    private void HandleDiceRolled(int roll)
    {
        if (!isTurn)
        {
            return;
        }

        Debug.Log("Handling dice roll");
        if(roll == 6 && atHomeUnits.Count>0)
        {
            foreach(var unit in atHomeUnits)
            {
                unit.EnableSelection();
            }
        }
        else
        {
            foreach(var unit in activeUnits)
            {
                unit.EnableSelection();
            }
        }
    }
}
