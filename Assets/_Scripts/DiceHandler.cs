using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceHandler : MonoBehaviour
{
    [Header("Dice Settings")]
    [SerializeField] private int rollValue;
    [SerializeField] private Dice _diceRoller;
    [SerializeField] private bool isRandom; 

    [Header("Button & Animation")]
    [SerializeField] private Button rollButton;
    [SerializeField] private Animator animator;

    [Header("Swipe Detection")]
    [SerializeField] private float maxSwipeSpeed = 1000f;
    private Vector2 swipeStartPosition;
    private float swipeStartTime;
    private bool canSwipe;

    [Header("Roll Force")]
    [SerializeField] private Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] private Vector2 _forceMax = new Vector2(0, 450);

    public static DiceHandler Instance { get; private set; }

    private void Awake()
    {
        Instance = this;
    }

    private void OnEnable()
    {
        rollButton.onClick.AddListener(RollDice);
        Dice.OnRoll += HandleRoll;
        GameManager.OnGameStateChanged += HandleStateChange;
    }

    private void OnDisable()
    {
        rollButton.onClick.RemoveListener(RollDice);
        Dice.OnRoll -= HandleRoll;
    }

    private void Start()
    {
        TurnManager.Instance.OnTurnChanged += EnableDice;
    }

    private void HandleStateChange(GameStates state)
    {
        canSwipe = state == GameStates.ROLL;
    }

    private void Update()
    {
        if (!canSwipe)
            return;

        // Detect swipe start
        if (Input.GetMouseButtonDown(0))
        {
            swipeStartPosition = Input.mousePosition;
            swipeStartTime = Time.time;
        }

        // Detect swipe end
        if (Input.GetMouseButtonUp(0))
        {
            Vector2 swipeEndPosition = Input.mousePosition;
            Vector2 swipeDirection = swipeEndPosition - swipeStartPosition;
            float swipeDuration = Time.time - swipeStartTime;
            float swipeDistance = swipeDirection.magnitude;
            float swipeSpeed = swipeDistance / swipeDuration;

            // Calculate force based on swipe speed
            float forcePercentage = Mathf.Clamp01(swipeSpeed / maxSwipeSpeed);
            float forceX = Mathf.Lerp(_forceMin.x, _forceMax.x, forcePercentage);
            float forceY = Mathf.Lerp(_forceMin.y, _forceMax.y, forcePercentage);
            Vector2 force = new Vector2(forceX, forceY);

            // Roll the dice with calculated force
            _diceRoller.SetRollForce(force);
            RollDice();
        }
    }

    private void RollDice()
    {
        if (isRandom)
        {
            //rollValue = Random.Range(1, 7);
            _diceRoller.RollDie();
        }
        else
        {
            _diceRoller.RollDie(rollValue);
        }
        
    }
    private void EnableDice(Home home)
    {
        canSwipe = true;
    }

    private void HandleRoll(int value)
    {
        canSwipe = false;
        Debug.LogError($"You Rolled a {value}");
        rollValue = value;
    }

    public int GetDiceRoll()
    {
        return rollValue;
    }

    /*private void RollDice()
    {
        _diceRoller.RollDie();
    }*/
}
