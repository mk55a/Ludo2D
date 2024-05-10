using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Home : MonoBehaviour
{
    [SerializeField] private List<UnitHolderBase> unitHolders;
    [SerializeField] private Color homeColor;
    [SerializeField] private GameObject unitPrefab;
    [SerializeField] private Animator animator;

    public List<Unit> activeUnits;
    public List<Unit> endUnits; 
    public List<Unit> atHomeUnits;
    public List<Unit> finishedUnits;

    public bool isTurn;
    
    private void Start()
    {
        activeUnits = new List<Unit>();
        atHomeUnits = new List<Unit>();
        endUnits = new List<Unit>();
        finishedUnits = new List<Unit>();

        OldDice.Instance.OnDiceRolled += HandleDiceRolled;
        Dice.OnRoll += HandleDiceRolled;

        InitiateUnits();
        ChangeTurn();
    }


    private void InitiateUnits()
    {
        foreach (UnitHolderBase holders in unitHolders)
        {
            Unit unit = holders.InstantiateUnit(unitPrefab);

            unit.OnStateChanged += HandleUnitStateChanged;
            unit.OnSelectionHandled += DisableUnitSelection;

        }
    }

    private UnitHolderBase FindEmptyUnitHolder()
    {
        foreach(UnitHolderBase holders in unitHolders)
        {
            if (holders.IsEmpty()) return holders;
            

        }
        return null;
    }

    private void HandleUnitStateChanged(Unit unit , UnitState newState)
    {
        switch (newState)
        {
            case UnitState.HOME:
                HandleUnitAtHome(unit);
                
                break;

            case UnitState.ONBOARD:
                HandleUnitOnBoard(unit);

                break;

            case UnitState.ONEND:
                HandleUnitOnEnd(unit);
                break;

            case UnitState.FINISH:
                HandleUnitFinished(unit);

                break;


        }
    }

    
    private void ChangeTurn()
    {
        animator.SetTrigger("IsTurn");
    }


    private void HandleDiceRolled(int roll)
    {
        if (!isTurn)
        {
            DisableUnitSelection();
            return;
        }

        //Debug.Log("Handling dice roll");

        if(activeUnits.Count == 0 &&endUnits.Count==0 && roll != 6) // or also if active Units are close to end and this dice roll won't let it move. 
        {
            TurnManager.Instance.EndTurn(false);
            return;
        }
        else if (roll == 6)
        {
            EnableHomeUnitSelection();
            EnableActiveUnitSelection();
        }
        else
        {
            //Debug.Log("Enablign Active units");
            EnableActiveUnitSelection();
            EnableEndUnitSelection();
        }

        GameManager.Instance.OnRollComplete();
    }

    

    
    private void EnableHomeUnitSelection()
    {
        if (atHomeUnits.Count != 0)
        {
            foreach (var unit in atHomeUnits)
            {
                unit.EnableSelection();
            }
        }
    }
    
    private void EnableActiveUnitSelection()
    {
        if (activeUnits.Count != 0)
        {
            foreach (var unit in activeUnits)
            {
                unit.EnableSelection();
            }
        }
    }


    private void EnableEndUnitSelection()
    {
        if (endUnits.Count != 0)
        {
            bool someUnitCanBeSelected = false;
            foreach (var unit in endUnits)
            {
                if (unit.CanBeSelected())
                {
                    someUnitCanBeSelected = true;
                    unit.EnableSelection();
                }

            }

            if (!someUnitCanBeSelected && activeUnits.Count ==0)
            {
                TurnManager.Instance.EndTurn(false);
            }
        }
    }

    private void DisableUnitSelection()
    {
        if (atHomeUnits.Count != 0)
        {
            foreach (var unit in atHomeUnits)
            {
                unit.DisableSelection();
            }
        }

        if (activeUnits.Count != 0)
        {
            foreach (var unit in activeUnits)
            {
                unit.DisableSelection();
            }
        }

        if(endUnits.Count!= 0)
        {
            foreach(var unit in endUnits)
            {
                unit.DisableSelection();
            }
        }
        GameManager.Instance.OnSelectionComplete();
    }

    public void ChangeTurn(bool newIsTurn)
    {
        isTurn = newIsTurn;
        animator.SetBool("IsTurn", isTurn);
    }



    // Handles unit state transition to HOME
    private void HandleUnitAtHome(Unit unit)
    {
        // Remove unit from other lists
        RemoveUnitFromLists(unit);

        // Add the unit to the 'atHomeUnits' list
        atHomeUnits.Add(unit);

        // Find an empty unit holder and add the unit if available
        UnitHolderBase holder = FindEmptyUnitHolder();
        if (holder != null)
        {
            holder.AddUnit(unit);
        }
    }

    // Handles unit state transition to ONBOARD
    private void HandleUnitOnBoard(Unit unit)
    {
        // Remove unit from other lists
        RemoveUnitFromLists(unit);

        // Add the unit to the 'activeUnits' list
        activeUnits.Add(unit);
    }

    // Handles unit state transition to ONEND
    private void HandleUnitOnEnd(Unit unit)
    {
        // Remove unit from other lists
        RemoveUnitFromLists(unit);

        // Add the unit to the 'endUnits' list
        endUnits.Add(unit);
    }

    // Handles unit state transition to FINISH
    private void HandleUnitFinished(Unit unit)
    {
        // Remove unit from other lists
        RemoveUnitFromLists(unit);

        // Add the unit to the 'finishedUnits' list
        finishedUnits.Add(unit);
    }

    // Helper method to remove unit from all lists
    private void RemoveUnitFromLists(Unit unit)
    {
        endUnits.Remove(unit);
        finishedUnits.Remove(unit);
        activeUnits.Remove(unit);
        atHomeUnits.Remove(unit);
    }
}
