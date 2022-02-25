using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class touchController : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 4f;
    [SerializeField] private float heightCoeff = 1f;
    private float startHeight = 0;

    [SerializeField] private FixedJoystick firstJoystick;
    [SerializeField] private FixedJoystick secondJoystick;

    [SerializeField] private GameObject hero;
    [SerializeField] private GameObject forBlock;

    [SerializeField] private Rigidbody2D rb;
    [SerializeField] private DistanceJoint2D joint;
    [SerializeField] private LineRenderer line;

    [SerializeField] private bool hookReady = false;
    [SerializeField] private bool pullBack = false;
    [SerializeField] private bool hookGrabbed = false;

    [SerializeField] private bool hookThrowed = false;

    [SerializeField] private Vector2 lastVecJoy2 = Vector2.zero;
    [SerializeField] private Vector2 hookGrabPoint = Vector2.zero;

    [SerializeField] private PlayerManager PM;

    private int wallLayer = 1 << 3;

    void Start()
    {
        startHeight = hero.transform.localScale.y;
    }

    void Update()
    {
        heightCoeff = 1f;

        firstJoystickCalc();
        secondJoystickCalc();

        calcBoxHeight();

        // rb.AddForce(Vector2.down * 8f, ForceMode2D.Force);
        rb.velocity += Vector2.down * 0.2f;

        if (hookGrabbed) line.SetPositions(new Vector3 [] {hero.transform.position, (Vector3)hookGrabPoint});
    }

    void calcBoxHeight()
    {
        var endScale = new Vector3(hero.transform.localScale.x, startHeight * heightCoeff, hero.transform.localScale.z);
        hero.transform.localScale = Vector3.Lerp(hero.transform.localScale, endScale, Time.deltaTime * 10f);
    }


    void firstJoystickCalc()
    {
        // if (Mathf.Abs(joystick.Horizontal) > 0.001f || Mathf.Abs(joystick.Vertical) > 0.001f)
        if (firstJoystick.Horizontal != 0 || firstJoystick.Vertical != 0)
        {
            if (hookGrabbed && joint.enabled)
            {
                if (Mathf.Abs(firstJoystick.Horizontal) > Mathf.Abs(firstJoystick.Vertical))
                {
                    Vector2 joystickVec = new Vector2(firstJoystick.Horizontal * moveSpeed, 0);
                    rb.AddForce(joystickVec * 2f, ForceMode2D.Force);
                }
                else
                {
                    if ((firstJoystick.Vertical < 0 && joint.distance < 10) || (firstJoystick.Vertical > 0 && joint.distance > 0.8f))
                    joint.distance += firstJoystick.Vertical * 0.08f * -1;
                }
            }
            else
            {
                if (Mathf.Abs(firstJoystick.Vertical) > Mathf.Abs(firstJoystick.Horizontal))
                {
                    if (firstJoystick.Vertical > 0)
                    {
                        RaycastHit2D hit = Physics2D.Raycast(hero.transform.position, Vector2.down, 0.35f, wallLayer);
                        if (hit.collider != null)
                        {
                            rb.velocity = Vector2.zero;
                            rb.AddForce(Vector2.up * 3f, ForceMode2D.Impulse);
                        }
                    }
                    else
                    {
                        // transform.localScale = Vector3.Lerp(transform.localScale, Vector3.one * 0.7f);

                        heightCoeff = 0.6f;
                    }
                }

                Vector2 joystickVec = new Vector2(firstJoystick.Horizontal * 6f, rb.velocity.y);
                rb.velocity = joystickVec;
                // rb.AddForce(joystickVec * 2f, ForceMode2D.Force);
                // rb.AddForce(Vector2.down * 5f, ForceMode2D.Force);
                // rb.velocity = joystickVec;
                // hero.transform.position += (Vector3)joystickVec * 0.03f;
                // rb.MovePosition(rb.position + joystickVec * 0.2f);
            }
        }
        else
        {
            if (!hookGrabbed || !joint.enabled)
            {
                Vector2 joystickVec = new Vector2(0, rb.velocity.y);
                rb.velocity = joystickVec;
            }
        }
    }

    void secondJoystickCalc()
    {
        if (secondJoystick.Horizontal != 0 || secondJoystick.Vertical != 0)
        {
            lastVecJoy2 = new Vector2(secondJoystick.Horizontal, secondJoystick.Vertical);
            // RaycastHit2D hit = Physics2D.Raycast(hero.transform.position, lastVecJoy2, Mathf.Infinity, wallLayer);

            // Debug.Log(hit.point);
            if (Mathf.Abs(secondJoystick.Horizontal) + Mathf.Abs(secondJoystick.Vertical) > 0.9) hookReady = true;
            else if (hookReady) hookReady = false;
            if (!pullBack) pullBack = true;

            // Debug.DrawLine(hero.transform.position, joystickVec, Color.red, 10f);
        }
        else if (pullBack)
        {
            throwHook();
        }
    }


    void throwHook()
    {
        pullBack = false;

        if (hookReady && !hookThrowed)
        {
            RaycastHit2D hit = Physics2D.Raycast(hero.transform.position, lastVecJoy2, Mathf.Infinity, wallLayer);
            
            if (hit.point != Vector2.zero)
            {
                if (hit.collider.gameObject.tag == "enemy")
                {
                    Destroy(hit.collider.gameObject);
                    PM.CoinRecount(3);
                    throwHook();
                }
                else
                {
                    if (hit.distance < 15f)
                    {
                        StartCoroutine(throwHookCoroutine(hit.point, hit.distance));

                        // hookGrabPoint = hit.point;
                        // joint.connectedAnchor = hookGrabPoint;
                        // joint.distance = hit.distance;
                        // line.SetPositions(new Vector3 [] {hero.transform.position, (Vector3)hookGrabPoint});
                        // joint.enabled = true;
                        // hookGrabbed = true;
                    }
                }
            }
        }
        else
        {
            line.SetPositions(new Vector3 [] {Vector3.zero, Vector3.zero});
            hookGrabbed = false;
            joint.enabled = false;
        }
    }

    IEnumerator throwHookCoroutine(Vector3 endPos, float distance)
    {
        hookThrowed = true;

        float duration = 0.1f * distance * 0.2f;
        var startPos = hero.transform.position;
        var currentPos = Vector3.zero;

        for(float t = 0; t < duration; t += Time.deltaTime)
        {
            currentPos = Vector3.Lerp(startPos, endPos, t / duration);
            line.SetPositions(new Vector3 [] {hero.transform.position, currentPos});
            yield return null;
        }

        hookGrabPoint = endPos;
        joint.connectedAnchor = hookGrabPoint;

        joint.distance = (hero.transform.position - endPos).magnitude;
        line.SetPositions(new Vector3 [] {hero.transform.position, (Vector3)hookGrabPoint});
        joint.enabled = true;
        hookGrabbed = true;

        hookThrowed = false;
    }
}
