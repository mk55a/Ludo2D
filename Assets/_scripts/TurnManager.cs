using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour
{
    public static TurnManager Instance {  get; private set; }   

    
    private List<Home> inGamePlayers;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

     

}
