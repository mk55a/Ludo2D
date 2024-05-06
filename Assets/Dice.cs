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
    [SerializeField] Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] Vector2 _forceMax = new Vector2(0, 450);
    [SerializeField] bool _usePhysics = true;
    [Tooltip("Roll time only appies when not using physics")]
    [SerializeField] float _rollDuration = 2f;

    static readonly int RollingAnimation = Animator.StringToHash("RollDice");
    static readonly int[] ResultAnimations =
    {
        //
        Animator.StringToHash("RollOnOne"),
        Animator.StringToHash("RollOnTwo"),
        /*Animator.StringToHash("LandOnThree"),
        Animator.StringToHash("LandOnFour"),
        Animator.StringToHash("LandOnFive"),
        Animator.StringToHash("LandOnSix"),*/

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
    }

    Vector2 GetRollForce()
    {
        Vector2 force = new Vector2();
        force.x = UnityEngine.Random.Range(_forceMin.x, _forceMax.x);
        force.y = UnityEngine.Random.Range(_forceMin.y, _forceMax.y);
        Debug.LogError(force);
        return force ;
    }


   

}
/* public void RollDice()
    {
        
        ApplyForce();
        animatingRenderer.gameObject.SetActive(true);
        resultRenderer.gameObject.SetActive(false);
        StartCoroutine(AnimateDice());
    }
    private void ApplyForce()
    {
        // Reset velocity and angular velocity before applying force
        rb.velocity = Vector2.zero;
        rb.angularVelocity = 0f;

        // Apply force to simulate the dice being thrown
        rb.AddForce(new Vector2(UnityEngine.Random.Range(-5f, 5f), UnityEngine.Random.Range(10f, 15f)), ForceMode2D.Impulse);
    }
    private IEnumerator AnimateDice()
    {
        float elapsedTime = 0f;
        int currentIndex = 0;
        float animationSpeed = initialAnimationSpeed;

        while (true)
        {
            animatingRenderer.sprite = allSprites[currentIndex];
            currentIndex = (currentIndex + 1) % allSprites.Count;

            // Accelerate the animation speed gradually
            elapsedTime += Time.deltaTime;
            animationSpeed = Mathf.Lerp(initialAnimationSpeed, finalAnimationSpeed, elapsedTime / accelerationTime);

            yield return new WaitForSeconds(animationSpeed);
        }
    }

    public void StopAnimation(int resultIndex)
    {
        StopAllCoroutines();
        animatingRenderer.gameObject.SetActive(false);
        resultRenderer.sprite = resultSprites[resultIndex];
        resultRenderer.gameObject.SetActive(true);
    }*/