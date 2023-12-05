using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BallManager : MonoBehaviour
{
    private Vector2 startP, endP;
    private GameObject ball;
    private Rigidbody ballRb;
    private LineRenderer lr;
    private bool isDragging;
    private float dragPower = 2;
    private GameManager gameManager;
    void Start()
    {
        ball = gameObject;
        ballRb = ball.GetComponent<Rigidbody>();
        ballRb.isKinematic = true;
        lr = GetComponent<LineRenderer>();
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
    }
    void Update()
    {
        if (!gameManager.isPhaseTransitioning && ballRb.isKinematic)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startP = Input.mousePosition;
                isDragging = true;
                lr.startColor = Color.white;
                lr.endColor = new Color(1, 1, 1, 0);
            }
            if (Input.GetMouseButtonUp(0) && startP != Vector2.zero)
            {
                endP = Input.mousePosition;
                Vector3 dir = startP - endP;
                dir = new Vector3(dir.x, dir.y, 0);
                ballRb.isKinematic = false;
                ballRb.AddForce(dir * ballRb.mass * dragPower);
                isDragging = false;
                StartCoroutine(FadeLine());
            }
            if (isDragging)
            {
                lr.SetPosition(0, ball.transform.position);
                lr.SetPosition(1, ball.transform.position + (Input.mousePosition - new Vector3(startP.x, startP.y, 0)) * 0.02f);
            }
        }
    }
    IEnumerator FadeLine()
    {
        float t = 0;
        while (t < 1)
        {
            t += Time.fixedDeltaTime;
            lr.startColor = Color.Lerp(Color.white, new Color(1, 1, 1, 0), t);
            yield return new WaitForFixedUpdate();
        }
        yield break;
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.tag == "StageSelectBlock") GetComponent<Collider>().isTrigger = true;
    }
}
