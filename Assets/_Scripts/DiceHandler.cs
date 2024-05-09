using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceHandler : MonoBehaviour
{
    [SerializeField] private int rollValue;
    private int diceRoll;

    [SerializeField] Button rollButton;
    [SerializeField] Dice _diceRoller;
    [SerializeField] private float maxSwipeSpeed = 1000f;
    private Vector2 swipeStartPosition;
    private float swipeStartTime;
    [SerializeField]
    private Animator animator;
    [SerializeField]
    private bool canSwipe; 

    [SerializeField] Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] Vector2 _forceMax = new Vector2(0, 450);

    public static DiceHandler Instance { get; private set; }
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
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
        if(state == GameStates.ROLL)
        {
            canSwipe = true;
            //animator.SetBool("CanThrowDice", true);
        }
        else
        {
            canSwipe = false;
            //animator.SetBool("CanThrowDice", false); 
        }
    }


    private void Update()
    {
        if (!canSwipe)
        {
            return;
        }


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
            float swipeDistance = swipeDirection.magnitude; //Vector2.Distance(swipeStartPosition, swipeEndPosition);
            float swipeSpeed = swipeDistance / swipeDuration;

            // Calculate force based on swipe speed
            float forcePercentage = Mathf.Clamp01(swipeSpeed / maxSwipeSpeed); // Assuming maxSwipeSpeed is predefined
            float forceX = Mathf.Lerp(_diceRoller._forceMin.x, _diceRoller._forceMax.x, forcePercentage);
            float forceY = Mathf.Lerp(_diceRoller._forceMin.y, _diceRoller._forceMax.y, forcePercentage);
            Vector2 force = new Vector2(forceX, forceY);

            //Vector2 force = Mathf.Lerp(_diceRoller._forceMin, _diceRoller._forceMax, forcePercentage);

            // Roll the dice with calculated force
            _diceRoller.SetRollForce(force);
            _diceRoller.RollDie(rollValue);
        }
    }

    void EnableDice(Home home)
    {
        canSwipe = true;
    }
    void HandleRoll(int obj)
    {
        canSwipe = false;
        Debug.LogError($"You Rolled a {obj}");
        diceRoll = obj;
    }
    public int GetDiceRoll()
    {
        return diceRoll;
    }
    void RollDice()
    {
        _diceRoller.RollDie();
    }
}
