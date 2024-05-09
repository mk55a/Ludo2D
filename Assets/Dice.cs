using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class Dice : MonoBehaviour
{
    public static event Action<int> OnRoll;

    public int Result { get; private set; }

    private Rigidbody2D rb;
    [SerializeField]Animator animator;
    [SerializeField] public Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] public Vector2 _forceMax = new Vector2(0, 450);
    [SerializeField] bool _usePhysics = true;
    [Tooltip("Roll time only appies when not using physics")]
    [SerializeField] float _rollDuration = 2f;

    private Vector2 rollFore; 
    static readonly int RollingAnimation = Animator.StringToHash("RollDice");
    static readonly int[] ResultAnimations =
    {
        //
        Animator.StringToHash("RollOnOne"),
        Animator.StringToHash("RollOnTwo"),
        Animator.StringToHash("RollOnThree"),
        Animator.StringToHash("RollOnFour"),
        Animator.StringToHash("RollOnFive"),
        Animator.StringToHash("RollOnSix"),

    };


    bool isRolling;
    float timeRemaining;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
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
        Debug.LogError("Collided With Table " +rb.velocity);
        if (rb.velocity.sqrMagnitude > 10f) return;
        FinishRolling();
    }

    public void RollDie(int value = 0)
    {
        Result = value == 0 ? UnityEngine.Random.Range(0, ResultAnimations.Length) : value;
        if (_usePhysics)
        {
            RollWithPhysics();
        }
        else
        {
            RollWithoutPhysics();
        }
    }


    void RollWithPhysics()
    {
        rb.isKinematic = false;
       
        rb.AddForce(GetRollForce(), ForceMode2D.Impulse);
        animator.SetTrigger(RollingAnimation);
        isRolling = true; 
    }

    void RollWithoutPhysics()
    {
        animator.SetTrigger(RollingAnimation);
        isRolling = true;
        timeRemaining = _rollDuration;
    }

    void FinishRolling()
    {
        Debug.LogError("Finishing Rolling");
        isRolling = false;
        rb.isKinematic = true;
        rb.velocity = Vector2.zero;
        animator.SetTrigger(ResultAnimations[Result]);

        OnRoll?.Invoke(Result);
        GameManager.Instance.OnRollComplete();
    }

    Vector2 GetRollForce()
    {
        Vector2 force = new Vector2();
        force.x = UnityEngine.Random.Range(_forceMin.x, _forceMax.x);
        force.y = UnityEngine.Random.Range(_forceMin.y, _forceMax.y);
        Debug.LogError(force);
        return force ;
    }

    public void SetRollForce(Vector2 force)
    {
        rollFore = force; 
    }

   

}
