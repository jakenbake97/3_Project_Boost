using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rocket : MonoBehaviour
{
    Rigidbody rigidbody;
    public GameObject rudder1;
    public GameObject rudder2;
    AudioSource audioSource;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        ProcessInput();
    }

    private void ProcessInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidbody.AddRelativeForce(Vector3.up);
            if (!audioSource.isPlaying) //prevents audio from layering
            {
                audioSource.Play();
            }
        }
        else
        {
            audioSource.Stop();
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward);
            rudder1.transform.Rotate(-Vector3.forward * 0.3f);
            rudder2.transform.Rotate(-Vector3.forward * 0.3f);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward);
            rudder1.transform.Rotate(Vector3.forward * 0.3f);
            rudder2.transform.Rotate(Vector3.forward * 0.3f);
        }

    }
}
