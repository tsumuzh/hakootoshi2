using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBlockManager : MonoBehaviour
{
    private bool isHit = false;
    private void OnCollisionEnter(Collision other)
    {
        if (!isHit)
        {
            transform.localScale *= 3;
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/break");
            isHit = true;
        }
    }
}
