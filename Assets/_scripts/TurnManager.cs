using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance { get; private set; }

    [Header("Turn Management")]
    [SerializeField] private List<Home> inGameHomes;
    [SerializeField] private float turnDelayDuration;

    private int currentHomeIndex = 0;

    public event Action<Home> OnTurnChanged;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        InitializeTurns();
        Unit.OnTraversalComplete += TurnDelay;
    }

    private void InitializeTurns()
    {
        if (inGameHomes.Count == 0)
        {
            Debug.LogError("No homes added to the turn Manager!");
            return;
        }

        currentHomeIndex = 0;
        StartTurn();
    }

    private void StartTurn()
    {
        StopCoroutine(TurnDelayCoroutine());
        foreach (var home in inGameHomes)
        {
            bool isCurrentHome = (home == inGameHomes[currentHomeIndex]);
            home.ChangeTurn(isCurrentHome);
        }

        OnTurnChanged?.Invoke(inGameHomes[currentHomeIndex]);
    }

    public void EndTurn(bool isSixRolled)
    {
        inGameHomes[currentHomeIndex].ChangeTurn(false);
        
        if (!isSixRolled)
        {
            
            currentHomeIndex = (currentHomeIndex + 1) % inGameHomes.Count;
            Debug.LogError("Changing turn to " + currentHomeIndex);
        }

        StartTurn();
    }

    public void TurnDelay()
    {
        StartCoroutine(TurnDelayCoroutine());
    }

    private IEnumerator TurnDelayCoroutine()
    {
        yield return new WaitForSeconds(turnDelayDuration);
        GameManager.Instance.OnMovementComplete();
    }
}
