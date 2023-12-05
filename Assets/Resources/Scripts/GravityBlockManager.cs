using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityBlockManager : MonoBehaviour
{
    private bool isHit = false;
    [SerializeField] Vector3 gravity;
    private void OnCollisionEnter(Collision other)
    {
        if (!isHit)
        {
            Physics.gravity = gravity * 9.81f;
            GetComponent<AudioSource>().clip = Resources.Load<AudioClip>("Sounds/break");
            isHit = true;
        }
    }
}
