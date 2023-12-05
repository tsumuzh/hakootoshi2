using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartStageSelectBlockManager : MonoBehaviour
{
    private GameManager gameManager;
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
            gameManager.OnStartStageSelectBlockFallen();
            gameObject.SetActive(false);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        gameManager.isPhaseTransitioning = true;
        GetComponent<Rigidbody>().useGravity = true;
    }
}
