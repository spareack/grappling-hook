using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class enemy : MonoBehaviour
{
    [SerializeField] private float pathWidth = 0f;
    [SerializeField] private Rigidbody2D rb;

    private float multiplyCoeff = 0;

    [SerializeField] private float leftSide;
    [SerializeField] private float rightSide;

    void Start()
    {
        if (pathWidth == 0) pathWidth = 2f;
        multiplyCoeff = Random.Range(2, 6);

        leftSide = transform.position.x - pathWidth;
        rightSide = transform.position.x + pathWidth;

        StartCoroutine((Random.Range(0, 2) > 0) ? walkToLeftSide() : walkToRightSide());
    }

    IEnumerator walkToLeftSide()
    {
        while(transform.position.x > leftSide) 
        {
            // rb.AddForce(Vector2.left * multiplyCoeff, ForceMode2D.Force);
            rb.velocity = Vector2.left * multiplyCoeff;
            yield return null;
        }
        StartCoroutine(walkToRightSide());
    }

    IEnumerator walkToRightSide()
    {
        while(transform.position.x < rightSide) 
        {
            // rb.AddForce(Vector2.right * multiplyCoeff, ForceMode2D.Force);
            rb.velocity = Vector2.right * multiplyCoeff;
            yield return null;
        }
        StartCoroutine(walkToLeftSide());
    }
}
