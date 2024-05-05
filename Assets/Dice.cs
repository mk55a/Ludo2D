using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class Dice : MonoBehaviour
{
    public event Action<int> OnDiceRolled; 
    public static Dice Instance { get; private set; }

    [SerializeField] private int value;
    [SerializeField] private Button diceButton;

    private int roll;

    private void Awake()
    {
        if(Instance == null)
        {
            Instance = this; 
        }
    }

    private void Start()
    {
        diceButton.onClick.AddListener(TestDiceRoll);
    }
    public void RollRandom()
    {

    }

    public void TestDiceRoll()
    {
        Debug.Log("DICE ROLLed");
        roll = value;
        OnDiceRolled?.Invoke(roll);
    }

    public int GetRoll()
    {
        return roll;
    }

}
