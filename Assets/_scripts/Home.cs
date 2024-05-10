using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Home : MonoBehaviour
{
    [Header("Unit Settings")]
    [SerializeField] private List<UnitHolderBase> unitHolders;
    [SerializeField] private GameObject unitPrefab;

    [Header("Turn Settings")]
    [SerializeField] private Animator animator;
    public bool isTurn;

    [Header("Unit Lists")]
    public List<Unit> activeUnits = new List<Unit>();
    public List<Unit> endUnits = new List<Unit>();
    public List<Unit> atHomeUnits = new List<Unit>();
    public List<Unit> finishedUnits = new List<Unit>();

    private void Start()
    {
        // Initialize unit lists
        InitUnitLists();

        // Subscribe to dice events
        Dice.OnRoll += HandleDiceRolled;

        // Initiate units
        InitiateUnits();

        // Change turn
        ChangeTurn();
    }

    private void InitUnitLists()
    {
        activeUnits.Clear();
        atHomeUnits.Clear();
        endUnits.Clear();
        finishedUnits.Clear();
    }

    private void InitiateUnits()
    {
        foreach (UnitHolderBase holder in unitHolders)
        {
            Unit unit = holder.InstantiateUnit(unitPrefab);
            unit.OnStateChanged += HandleUnitStateChanged;
            unit.OnSelectionHandled += DisableUnitSelection;
        }
    }

    private UnitHolderBase FindEmptyUnitHolder()
    {
        foreach (UnitHolderBase holder in unitHolders)
        {
            if (holder.IsEmpty())
                return holder;
        }
        return null;
    }

    private void HandleUnitStateChanged(Unit unit, UnitState newState)
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

        if (activeUnits.Count == 0 && endUnits.Count == 0 && roll != 6)
        {
            TurnManager.Instance.TurnDelay();
            return;
        }
        else if (roll == 6)
        {
            EnableHomeUnitSelection();
            EnableActiveUnitSelection();
        }
        else
        {
            EnableActiveUnitSelection();
            EnableEndUnitSelection();
        }

        GameManager.Instance.OnRollComplete();
    }

    private void EnableHomeUnitSelection()
    {
        foreach (var unit in atHomeUnits)
            unit.EnableSelection();
    }

    private void EnableActiveUnitSelection()
    {
        foreach (var unit in activeUnits)
            unit.EnableSelection();
    }

    private void EnableEndUnitSelection()
    {
        foreach (var unit in endUnits)
        {
            if (unit.CanBeSelected())
                unit.EnableSelection();
        }

        if (!endUnits.Any(unit => unit.CanBeSelected()) && activeUnits.Count == 0)
            TurnManager.Instance.TurnDelay();
    }

    private void DisableUnitSelection()
    {
        foreach (var unit in atHomeUnits.Concat(activeUnits).Concat(endUnits))
            unit.DisableSelection();

        GameManager.Instance.OnSelectionComplete();
    }

    public void ChangeTurn(bool newIsTurn)
    {
        isTurn = newIsTurn;
        animator.SetBool("IsTurn", isTurn);
    }

    private void HandleUnitAtHome(Unit unit)
    {
        RemoveUnitFromLists(unit);
        atHomeUnits.Add(unit);

        UnitHolderBase holder = FindEmptyUnitHolder();
        if (holder != null)
            holder.AddUnit(unit);
    }

    private void HandleUnitOnBoard(Unit unit)
    {
        RemoveUnitFromLists(unit);
        activeUnits.Add(unit);
    }

    private void HandleUnitOnEnd(Unit unit)
    {
        RemoveUnitFromLists(unit);
        endUnits.Add(unit);
    }

    private void HandleUnitFinished(Unit unit)
    {
        RemoveUnitFromLists(unit);
        finishedUnits.Add(unit);
    }

    private void RemoveUnitFromLists(Unit unit)
    {
        endUnits.Remove(unit);
        finishedUnits.Remove(unit);
        activeUnits.Remove(unit);
        atHomeUnits.Remove(unit);
    }
}
