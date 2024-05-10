
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    public static event Action OnTraversalComplete;
    public event Action<Unit, UnitState> OnStateChanged;
    public event Action OnSelectionHandled;

    [SerializeField] private Color unitColor;
    [SerializeField] private Button selectionButton;
    [SerializeField] private GameObject selectionUI;
    [SerializeField] private float speed;

    public UnitState currentState;
    public List<Tile> tilesTraversed;
    public List<Tile> tilesToBeTraversed;

    private bool canMove;
    private int currentTraversalIndex;
    private Animator animator;
    private Vector3 originalScale;

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
        originalScale = transform.localScale;
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
        else if (currentState == UnitState.ONEND)
        {
            GetEndTilesToTraverse();
        }
        TraverseTiles();
    }

    public bool CanBeSelected()
    {
        return GetTilesTraversedCount() + DiceHandler.Instance.GetDiceRoll() <= 57;
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
            Debug.Log(tilesTraversed.Count + " Traversing home");
            yield break;
        }
        for (currentTraversalIndex = GetTilesTraversedCount() - 2; currentTraversalIndex >= 0; currentTraversalIndex--)
        {
            tilesTraversed[currentTraversalIndex].RemoveUnit(this);
            transform.SetParent(TileManager.Instance.inMovementParent.transform);
            ChangeScale();

            Vector3 targetPosition = tilesTraversed[currentTraversalIndex].transform.position;
            float distance = Vector3.Distance(transform.position, targetPosition);
            float journeyTime = 0.2f;
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
            yield return new WaitForSeconds(0.125f);
        }

        tilesTraversed.Clear();
        canMove = false;
        SetState(UnitState.HOME);
    }

    public void TraverseTiles()
    {
        if (tilesToBeTraversed.Count == 0) return;
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
        if (tilesTraversed.Count != 0)
        {
            tilesTraversed[tilesTraversed.Count - 1].RemoveUnit(this);
        }
        for (currentTraversalIndex = 0; currentTraversalIndex < tilesToBeTraversed.Count; currentTraversalIndex++)
        {
            transform.SetParent(TileManager.Instance.inMovementParent.transform);
            ChangeScale();

            Vector3 targetPosition = tilesToBeTraversed[currentTraversalIndex].transform.position;
            float distance = Vector3.Distance(transform.position, targetPosition);
            float journeyTime = 0.2f;
            float elapsedTime = 0f;
            Vector3 startingPosition = transform.position;

            while (elapsedTime < journeyTime)
            {
                elapsedTime += Time.deltaTime;
                transform.position = Vector3.Lerp(startingPosition, targetPosition, elapsedTime / journeyTime);
                yield return null;
            }

            SetToOriginalSize();
            tilesTraversed.Add(tilesToBeTraversed[currentTraversalIndex]);
            yield return new WaitForSeconds(0.325f);
        }

        tilesToBeTraversed[tilesToBeTraversed.Count - 1].AddUnit(this);
        tilesToBeTraversed.Clear();
        canMove = false;
    }

    public void GetFirstTileOnBoard()
    {
        tilesToBeTraversed = TileManager.Instance.GetStartTile(this);
        if (tilesToBeTraversed != null)
        {
            SetState(UnitState.ONBOARD);
        }
    }

    public void GetEndTilesToTraverse()
    {
        tilesToBeTraversed = TileManager.Instance.GetUnitsEndTraversal(this);
    }

    public void GetTilesToTraverse()
    {
        tilesToBeTraversed = TileManager.Instance.GetUnitsTileTraversal(this);
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
        return GetTilesTraversedCount() + DiceHandler.Instance.GetDiceRoll() <= 57;
    }

    private void SetToOriginalSize()
    {
        transform.localScale = originalScale;
    }

    private void ChangeScale()
    {
        transform.localScale *= 1.2f;
    }
}
