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
    public List<Unit> atHomeUnits;
    public List<Unit> finishedUnits;

    public bool isTurn;
    
    private void Start()
    {
        activeUnits = new List<Unit>();
        atHomeUnits = new List<Unit>();
        finishedUnits = new List<Unit>();
        OldDice.Instance.OnDiceRolled += HandleDiceRolled;
        Dice.OnRoll += HandleDiceRolled;
        foreach(UnitHolderBase holders in unitHolders)
        {
            Unit unit =holders.InstantiateUnit(unitPrefab);

            unit.OnStateChanged += HandleUnitStateChanged;
            unit.OnSelectionHandled += OnMoveComplete;
           
        }
        ChangeTurn();
    }


    private void Update()
    {
        /*if(isTurn)
        {
           animator.i
        }
        else
        {
            gameObject.SetActive(false);
        }*/
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

        Debug.Log("Handling dice roll");
        
        if(roll == 6)
        {
            EnableHomeUnitSelection();
            EnableActiveUnitSelection();
        }
        else
        {
            EnableActiveUnitSelection();
        }
    }

    private void OnMoveComplete()
    {
        DisableUnitSelection();
        OldDice.Instance.diceButton.interactable = true; 
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
    }

    public void ChangeTurn(bool newIsTurn)
    {
        isTurn = newIsTurn;
        animator.SetBool("IsTurn", isTurn);
    }
}
