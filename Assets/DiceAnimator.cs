using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiceAnimator : MonoBehaviour
{
    public List<Sprite> allSprites; // List of all dice face sprites
    public List<Sprite> resultSprites; // List of resulting face sprites
    public float initialAnimationSpeed = 0.1f; // Initial speed of the animation
    public float finalAnimationSpeed = 0.5f; // Final speed of the animation
    public float accelerationTime = 2f; // Time taken to reach final speed
    public Image animatingRenderer;
    public Image resultRenderer;
    //public SpriteRenderer animatingRenderer; // Sprite renderer for animating dice
    //public SpriteRenderer resultRenderer; //52,112,116,120,176
    //252-5 , 248-6,244-2,240-1,177-3,59-4

    private Rigidbody2D rb;
    [SerializeField] bool _usePhysics= true;
    [SerializeField] Vector2 _forceMin = new Vector2(0, 350);
    [SerializeField] Vector2 _forceMax = new Vector2(0, 450);
    [SerializeField] float _rollDuration = 2f;

    public int Result { get; private set; } 


    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    public void RollDie(int value = 0)
    {
        Result = value == 0 ? UnityEngine.Random.Range(1, resultSprites.Count) : value;
        if (_usePhysics)
        {
            //RollWithPhysics();
        }
        else
        {
            //RollWithoutPhysics();
        }
    }

    public void RollDice()
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
        rb.AddForce(new Vector2(Random.Range(-5f, 5f), Random.Range(10f, 15f)), ForceMode2D.Impulse);
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
    }

}
