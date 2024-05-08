using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [SerializeField]
    private Home[] homes = new Home[4];



    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this;
        }
    }

    public void SelectColor()
    {
        //TurnManager
    }


}
