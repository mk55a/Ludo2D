using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance {  get; private set; }

    [SerializeField]
    private List<Home> inGameHomes;

    private int currentHomeIndex = 0;


    public event Action<Home> OnTurnChanged; 


    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    private void Start()
    {
        if(inGameHomes.Count == 0)
        {
            Debug.LogError("No homes added to the turn Manager !");
            return;
        }

        currentHomeIndex = 0;
        StartTurn();
    }

    private void StartTurn()
    {
        for(int i = 0; i< inGameHomes.Count; i++)
        {
            if(i == currentHomeIndex)
            {
                inGameHomes[i].isTurn = true;
                
            }
            else
            {
                inGameHomes[i].isTurn = false;
                
            }
        }

        OnTurnChanged?.Invoke(inGameHomes[currentHomeIndex]);
    }

    public void EndTurn(bool isSixRolled)
    {
        inGameHomes[currentHomeIndex].isTurn = false;
        

        if (!isSixRolled)
        {
            currentHomeIndex = (currentHomeIndex + 1) % inGameHomes.Count;
        }

        StartTurn();
    }

}
