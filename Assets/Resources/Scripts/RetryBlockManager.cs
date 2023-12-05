using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RetryBlockManager : MonoBehaviour
{
    GameManager gameManager;
    private AudioSource audioSource;
    void Start()
    {
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (transform.position.y < -7)
        {
            gameManager.StartGame(gameManager.currentStageNumber);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        gameManager.isPhaseTransitioning = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb.useGravity) rb.useGravity = true;
    }
}
