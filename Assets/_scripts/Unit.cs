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


    private List<Tile> tilesTraversed;

    private List<Tile> tilesToBeTraversed;
    private bool canMove;
    int currentTraversalIndex;
    private void Start()
    {
        tilesTraversed = new List<Tile>();
        tilesToBeTraversed = new List<Tile>();
        SetState(UnitState.HOME);
        currentTraversalIndex = 0;
    }

    private void Update()
    {
        if (!canMove) return;
        Debug.Log("Unit is moving");
        float step = speed * Time.deltaTime;
        transform.position = Vector3.MoveTowards(transform.position, tilesToBeTraversed[currentTraversalIndex].transform.position, step);
        transform.SetParent(tilesToBeTraversed[currentTraversalIndex].transform);
        if (Vector3.Distance(transform.position, tilesToBeTraversed[currentTraversalIndex].transform.position) < 0.001f)
        {
            Debug.LogWarning("Moved up by one");
            tilesTraversed.Add(tilesToBeTraversed[currentTraversalIndex]);
            currentTraversalIndex++;
            if (currentTraversalIndex == tilesToBeTraversed.Count)
            {
                currentTraversalIndex = 0;
                canMove = false;
                tilesToBeTraversed.Clear();
                //GameManager.instance.MoveEnd();
            }
        }
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
        HandleSelection();
        //selectionButton.onClick.AddListener(HandleSelection);
    }

    public void DisableSelection()
    {
        selectionButton.interactable = false;

        //selectionButton.onClick.RemoveAllListeners();
    }

    public void HandleSelection()
    {
        if (currentState == UnitState.HOME)
        {
            Debug.Log("Handling HOME STATE, Getting bLue Start");
            tilesToBeTraversed=  TileManager.Instance.GetBlueStart();
            if(tilesToBeTraversed!= null)
            {
                SetState(UnitState.ONBOARD);
            }

        }
        else if (currentState == UnitState.ONBOARD)
        {
            int roll = Dice.Instance.GetRoll();
            GetTilesToTraverse();
        }
        TraverseTiles();
    }

    public void TraverseTiles()
    {
        Debug.Log("Going to traverse tiles");
        if (tilesToBeTraversed.Count == 0) return;
        canMove = true;
        //StartCoroutine(TraversalTilesCoroutine());
    }

    public void GetTilesToTraverse()
    {
        tilesToBeTraversed = TileManager.Instance.GetUnitsTileTraversal(tilesTraversed[tilesTraversed.Count - 1], Dice.Instance.GetRoll());
    }

    /*public void MoveToTile(Tile tile)
    {

        Debug.LogError(tile.gameObject.name +"  "+ tile.transform.position);
        transform.SetParent(tile.transform);
        //Get the postion of the tile game object and move this game object there. 
        //StartCoroutine(MoveToTileCoroutine(tile.transform.position));
        tilesTraversed.Add(tile);
    }*/

    /*private IEnumerator MoveToTileCoroutine(Vector3 targetPosition)
    {
        float journeyLength = Vector3.Distance(transform.position, targetPosition);
        float startTime = Time.time;
        float duration = 1f; // Adjust duration as needed for desired speed
        
        while (true)
        {
            float distCovered = (Time.time - startTime) * duration;
            float fracJourney = distCovered / journeyLength;
            transform.position = Vector3.Lerp(transform.position, targetPosition, fracJourney);

            if (fracJourney >= 1f)
                break;

            yield return null;
        }
    }*/

    
   /* private IEnumerator TraversalTilesCoroutine()
    {
        Debug.LogWarning("No of tiles to be traversed : " + tilesToBeTraversed.Count);
        foreach (Tile tile in tilesToBeTraversed)
        {
            MoveToTile(tile);
            //Wait for a time frame before moving to the next tile in the list. 
            yield return new WaitForSeconds(1f);
        }
        Debug.LogError("Clearing Traversal to be");
        tilesToBeTraversed.Clear();
    }*/

    
}
