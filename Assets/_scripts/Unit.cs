using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public static event Action OnTraversalComplete; 
    public event Action<Unit, UnitState> OnStateChanged;
    public event Action OnSelectionHandled; 

    [SerializeField]
    private Color unitColor;

    [SerializeField]
    private Button selectionButton;
    [SerializeField]
    private GameObject selectionUI;

    [SerializeField]
    private float speed ;
    public UnitState currentState;


    public List<Tile> tilesTraversed;

    public List<Tile> tilesToBeTraversed;
    private bool canMove;
    int currentTraversalIndex;
    private Animator animator;

    Vector3 originalScale;
    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    private void Start()
    {
        tilesTraversed = new List<Tile>();
        tilesToBeTraversed = new List<Tile>();
        SetState(UnitState.HOME);
        currentTraversalIndex = 0;
        originalScale= transform.localScale;
    }

    public void SetState(UnitState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(this, currentState);
    }

    public void EnableSelection()
    {
        
        selectionButton.interactable = true;
        selectionUI.SetActive(true);
        animator.SetBool("CanBeSelected", true);
        //selectionButton.onClick.AddListener(HandleSelection);

        //HandleSelection();
    }

    public void DisableSelection()
    {
        selectionButton.onClick.RemoveListener(HandleSelection);
        selectionButton.interactable = false;
        selectionUI.SetActive(false);
        animator.SetBool("CanBeSelected", false);
    }

    public void HandleSelection()
    {
        OnSelectionHandled?.Invoke();
        
        if (currentState == UnitState.HOME)
        {
            GetFirstTileOnBoard();
        }
        else if (currentState == UnitState.ONBOARD)
        {
            GetTilesToTraverse();
        }
        else if(currentState == UnitState.ONEND)
        {
            GetEndTilesToTraverse();
        }

        TraverseTiles();
    }

    public bool CanBeSelected()
    {
        if(GetTilesTraversedCount() + DiceHandler.Instance.GetDiceRoll() > 57)
        {
            return false;
        }
        else
        {
            return true;
        }
    }

    public void UnitFinished()
    {
        SetState(UnitState.FINISH);
    }

    public void UnitIsOnEnd()
    {
        SetState(UnitState.ONEND);
    }

    public void TraverseBackToHome()
    {
        canMove = true;
        StartCoroutine(TraverseTilesToHomeCoroutine());
    }

    private IEnumerator TraverseTilesToHomeCoroutine()
    {
        if (!canMove || GetTilesTraversedCount() == 0)
        {
            Debug.Log(tilesTraversed.Count + "Traversing home");
            yield break;
        }
        for(currentTraversalIndex =GetTilesTraversedCount()-2; currentTraversalIndex>=0; currentTraversalIndex--)
        {
            tilesTraversed[currentTraversalIndex].RemoveUnit(this);
            transform.SetParent(TileManager.Instance.inMovementParent.transform);

            ChangeScale();
            Debug.LogError("INDEX : " + currentTraversalIndex +  " , " + GetTilesTraversedCount());
            Vector3 targetPosition = tilesTraversed[currentTraversalIndex].transform.position;

            float distance = Vector3.Distance(transform.position, targetPosition);
            float journeyTime = 0.2f; // Adjust the duration of movement (lower values for quicker movement)
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;

            while (elapsedTime < journeyTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / journeyTime);
                yield return null;
            }

            SetToOriginalSize();

            tilesTraversed.Remove(tilesTraversed[currentTraversalIndex]);   
            Debug.LogWarning("Moved down by one");


            yield return new WaitForSeconds(0.125f);
        }
        Debug.Log("UnitBackToHome");

        //Add it back to home.
        tilesTraversed.Clear();
        canMove = false;
        SetState(UnitState.HOME);
        

    }

    public void TraverseTiles()
    {
        if (tilesToBeTraversed.Count == 0) return;
        Debug.Log("Going to traverse tiles");
        canMove = true;
        StartCoroutine(TraversalTilesCoroutine());
        OnTraversalComplete?.Invoke();

    }

    private IEnumerator TraversalTilesCoroutine()
    {
        if (!canMove || tilesToBeTraversed.Count == 0)
        {
            yield break;
        }

        
        if(tilesTraversed.Count != 0)
        {
            tilesTraversed[tilesTraversed.Count - 1].RemoveUnit(this);
        }
        

        for(currentTraversalIndex =0; currentTraversalIndex<tilesToBeTraversed.Count; currentTraversalIndex++)
        {
            //Debug.LogError(tilesToBeTraversed.Count + " Current " + currentTraversalIndex);
            transform.SetParent(TileManager.Instance.inMovementParent.transform);

            ChangeScale();

            Vector3 targetPosition = tilesToBeTraversed[currentTraversalIndex].transform.position;
            
            float distance = Vector3.Distance(transform.position, targetPosition);
            float journeyTime = 0.2f; // Adjust the duration of movement (lower values for quicker movement)
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;

            while(elapsedTime < journeyTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime/journeyTime);   
                yield return null;
            }

            SetToOriginalSize();
            
            //transform.SetParent(tilesToBeTraversed[currentTraversalIndex].transform);


            tilesTraversed.Add(tilesToBeTraversed[currentTraversalIndex]);
            Debug.LogWarning("Moved up by one");

            
            yield return new WaitForSeconds(0.325f);

        }
        Debug.Log("MovementISDONE");
        
        tilesToBeTraversed[tilesToBeTraversed.Count - 1].AddUnit(this);
        tilesToBeTraversed.Clear();
        canMove = false;
        
    }

    public void GetFirstTileOnBoard()
    {
        tilesToBeTraversed = TileManager.Instance.GetStartTile(this);
        if (tilesToBeTraversed != null)
        {
            Debug.Log("HOME to ONBOARD, Getting bLue Start");
            SetState(UnitState.ONBOARD);
        }
    }
    
    public void GetEndTilesToTraverse()
    {
        tilesToBeTraversed = TileManager.Instance.GetUnitsEndTraversal(this);
        foreach (var tile in tilesToBeTraversed)
        {
            Debug.LogError(tile.gameObject.name + " " + tile.GetPositionIndex());
        }
    }

    public void GetTilesToTraverse()
    {
        //tilesToBeTraversed = TileManager.Instance.GetUnitsTileTraversal(tilesTraversed[tilesTraversed.Count - 1], DiceHandler.Instance.GetDiceRoll());//OldDice.Instance.GetRoll());//, GetTilesTraversedCount()); 
       
        tilesToBeTraversed = TileManager.Instance.GetUnitsTileTraversal(this);
        
        foreach(var tile in tilesToBeTraversed)
        {
            Debug.LogError(tile.gameObject.name + " "+ tile.GetPositionIndex());
        }
    }

    public int GetTilesTraversedCount()
    {
        return tilesTraversed.Count;
    }

    public Tile GetCurrentTile()
    {
        return tilesTraversed[tilesTraversed.Count - 1];
    }

    public Color GetUnitColor()
    {
        return unitColor;
    }

    public UnitState GetUnitState()
    {
        return currentState;
    }

    public bool CanUnitMove()
    {
        if(GetTilesTraversedCount() + DiceHandler.Instance.GetDiceRoll() > 57)
        {
            return false;
        }
        else
        {
            return true;
        }
    }
    
    private void SetToOriginalSize()
    {
        transform.localScale = originalScale;
        Debug.Log("RevertedBackToOrigianlSize");
    }

    private void ChangeScale()
    {
        transform.localScale *= 1.2f;
        Debug.Log("ChangedScale");
    }
}
