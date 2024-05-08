using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
public class Unit : MonoBehaviour
{
    public event Action<Unit, UnitState> OnStateChanged; 


    [SerializeField]
    private Color unitColor;

    [SerializeField]
    private Button selectionButton;

    [SerializeField]
    private float speed ;
    public UnitState currentState;


    public List<Tile> tilesTraversed;

    public List<Tile> tilesToBeTraversed;
    private bool canMove;
    int currentTraversalIndex;
    private Animator animator;

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
    }

    private void Update()
    {
       

        
    }

    public void SetState(UnitState newState)
    {
        currentState = newState;
        OnStateChanged?.Invoke(this, currentState);
    }

    public void EnableSelection()
    {
        Debug.Log("Can be selected " + currentState);
        selectionButton.interactable = true;
        selectionButton.onClick.AddListener(HandleSelection);

        //HandleSelection();
    }

    public void DisableSelection()
    {
        selectionButton.onClick.RemoveListener(HandleSelection);
        selectionButton.interactable = false;
    }

    public void HandleSelection()
    {
        if (currentState == UnitState.HOME)
        {
            GetFirstTileOnBoard();
        }
        else if (currentState == UnitState.ONBOARD)
        {
            //GetTilesToTraverse();
        }

        TraverseTiles();
    }

    public void TraverseTiles()
    {
        if (tilesToBeTraversed.Count == 0) return;
        Debug.Log("Going to traverse tiles");
        canMove = true;
        StartCoroutine(TraversalTilesCoroutine());
    }

    private IEnumerator TraversalTilesCoroutine()
    {
        if (!canMove)
        {
            yield break;
        }
        if (tilesTraversed.Count >= 51)
        {
            tilesToBeTraversed.Clear();
            //tilesToBeTraversed = 
        }
        for(currentTraversalIndex =0; currentTraversalIndex<tilesToBeTraversed.Count; currentTraversalIndex++)
        {
            transform.SetParent(TileManager.Instance.inMovementParent.transform);
            Vector3 originalScale = transform.localScale;
            transform.localScale *= 1.2f;

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
            
            transform.localScale = originalScale;
            transform.SetParent(tilesToBeTraversed[currentTraversalIndex].transform);


            tilesTraversed.Add(tilesToBeTraversed[currentTraversalIndex]);
            Debug.LogWarning("Moved up by one");

            
            yield return new WaitForSeconds(0.65f);

        }

        tilesToBeTraversed.Clear();
        canMove = false;
    }

    public void GetFirstTileOnBoard()
    {
        tilesToBeTraversed = TileManager.Instance.GetBlueStart();
        if (tilesToBeTraversed != null)
        {
            Debug.Log("HOME to ONBOARD, Getting bLue Start");
            SetState(UnitState.ONBOARD);
        }
    }
    
    public void GetTilesToTraverse(int traversedCount)
    {
        tilesToBeTraversed = TileManager.Instance.GetUnitsTileTraversal(tilesTraversed[tilesTraversed.Count - 1], OldDice.Instance.GetRoll());//, GetTilesTraversedCount()); 
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
    
}
