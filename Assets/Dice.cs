using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;


public class Dice : MonoBehaviour
{
    public static event Action<int> OnRoll;

    public int Result { get; private set; }

    [Header("Dice Settings")]
    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private Animator animator;
    [SerializeField] private bool _usePhysics = true;
    [SerializeField] private float _rollDuration = 2f;
    [SerializeField] private Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] private Vector2 _forceMax = new Vector2(0, 450);

    private Vector2 rollForce;
    private static readonly int RollingAnimation = Animator.StringToHash("RollDice");
    private static readonly int[] ResultAnimations = {
        Animator.StringToHash("RollOnOne"),
        Animator.StringToHash("RollOnTwo"),
        Animator.StringToHash("RollOnThree"),
        Animator.StringToHash("RollOnFour"),
        Animator.StringToHash("RollOnFive"),
        Animator.StringToHash("RollOnSix")
    };

    private bool isRolling;
    private float timeRemaining;

    private void Start()
    {
        rb.isKinematic = true;
    }

    private void Update()
    {
        if (!isRolling) return;
        timeRemaining -= Time.deltaTime;
        if (_usePhysics || timeRemaining > 0f) return;
        FinishRolling();
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (rb.velocity.sqrMagnitude > 10f) return;
        FinishRolling();
    }

    public void RollDie(int value = 0)
    {
        Result = value == 0 ? UnityEngine.Random.Range(0, ResultAnimations.Length + 1) : value;
        if (_usePhysics)
            RollWithPhysics();
        else
            RollWithoutPhysics();
    }

    private void RollWithPhysics()
    {
        rb.isKinematic = false;
        rb.AddForce(GetRollForce(), ForceMode2D.Impulse);
        animator.SetTrigger(RollingAnimation);
        isRolling = true;
    }

    private void RollWithoutPhysics()
    {
        animator.SetTrigger(RollingAnimation);
        isRolling = true;
        timeRemaining = _rollDuration;
    }

    private void FinishRolling()
    {
        isRolling = false;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger(ResultAnimations[Result - 1]);
        OnRoll?.Invoke(Result);
    }

    private Vector2 GetRollForce()
    {
        return rollForce;
    }

    public void SetRollForce(Vector2 force)
    {
        rollForce = force;
    }
}
