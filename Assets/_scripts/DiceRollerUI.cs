using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceRollerUI : MonoBehaviour
{
    [SerializeField] Button rollButton;
    [SerializeField] Dice _diceRoller;

    private void OnEnable()
    {
        rollButton.onClick.AddListener(RollDice);
        Dice.OnRoll += HandleRoll;

    }

    private void OnDisable()
    {
        rollButton.onClick.RemoveListener(RollDice);
        Dice.OnRoll -= HandleRoll;
    }

    void HandleRoll(int obj)
    {
        Debug.LogError($"You Rolled a {obj}");

    }

    void RollDice()
    {
        _diceRoller.RollDie();
    }
}
