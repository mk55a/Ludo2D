using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    public delegate void GameStateChangedHandler(GameStates newState);
    public static event GameStateChangedHandler OnGameStateChanged;

    private GameStates currentState;

    [SerializeField] private Home[] homes = new Home[4];

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        ChangeGameState(GameStates.ROLL);
    }

    public void ChangeGameState(GameStates newState)
    {
        currentState = newState;
        OnGameStateChanged?.Invoke(newState);
    }

    public void OnRollComplete()
    {
        if (currentState == GameStates.ROLL)
        {
            // Transition to movement state
            ChangeGameState(GameStates.SELECTION);
        }
    }

    public void OnMovementComplete()
    {
        if (currentState == GameStates.MOVEMENT)
        {
            // Transition to selection state
            ChangeGameState(GameStates.ROLL);
        }
    }

    public void OnSelectionComplete()
    {
        if (currentState == GameStates.SELECTION)
        {
            // Transition to next round or any other logic you need
            // For example, you can return to the roll state
            ChangeGameState(GameStates.MOVEMENT);
        }
    }
}

public enum GameStates
{
    ROLL, 
    MOVEMENT, 
    SELECTION
}
