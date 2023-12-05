using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockManager : MonoBehaviour
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
        if ((Physics.gravity.y < 0 && transform.position.y < -7)
        || (Physics.gravity.y > 0 && transform.position.y > 7)
        || (Physics.gravity.x < 0 && transform.position.x < -10)
        || (Physics.gravity.x > 0 && transform.position.x > 10))
        {

            gameManager.DecrementAndCheckBlockCount();
            Destroy(gameObject);

        }
    }
    private void OnCollisionEnter(Collision other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        Rigidbody rb = GetComponent<Rigidbody>();
        if (!rb.useGravity)
        {
            rb.useGravity = true;
            gameManager.blockCountInFalling++;
        }
    }
}
