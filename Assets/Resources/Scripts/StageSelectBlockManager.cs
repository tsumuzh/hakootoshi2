using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageSelectBlockManager : MonoBehaviour
{
    public int stageNumber;
    private GameManager gameManager;
    private AudioSource audioSource;
    void Start()
    {
        gameManager = GameObject.Find("Manager").GetComponent<GameManager>();
        audioSource = GetComponent<AudioSource>();
    }
    void Update()
    {
        if (transform.position.y < 3)
        {
            gameManager.StartGame(stageNumber);
            Destroy(gameObject);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        audioSource.PlayOneShot(audioSource.clip);
        gameManager.isPhaseTransitioning = true;
        GetComponent<Rigidbody>().useGravity = true;
        GameObject[] objs = GameObject.FindGameObjectsWithTag("StageSelectBlock");
        foreach (GameObject obj in objs)
        {
            if (obj != gameObject)
            {
                obj.GetComponent<Collider>().isTrigger = true;
                obj.GetComponent<Renderer>().material.color = new Color(1, 1, 1, 0.5f);
            }
        }
    }
}
