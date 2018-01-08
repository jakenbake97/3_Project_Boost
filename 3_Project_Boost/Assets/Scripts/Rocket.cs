using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    /* TODO: 
    *   somewhere stop sound on death
    *   parameterise time
    *   allow for more than 2 levels
    */

    Rigidbody rigidbody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] AudioClip mainEnigne;
    [SerializeField] AudioClip levelWin;
    [SerializeField] AudioClip deathExplosion;

    enum State { Alive, Dying, Transcending }
    State state = State.Alive;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    { //TODO: somewhere stop sound on death
        if (state == State.Alive)
        {
            RespondToRotateInput();
            RespondToThrustInput();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive) { return; } // ignore collisons
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                state = State.Transcending;
                audioSource.PlayOneShot(levelWin);
                Invoke("LoadNextLevel", 1f); // TODO: parameterise time
                break;
            default:
                state = State.Dying;
                audioSource.PlayOneShot(deathExplosion);
                Invoke("LoadFirstLevel", 1f); // TODO: parameterise time
                break;
        }
    }
    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        SceneManager.LoadScene(1); // TODO: allow for more than 2 levels

    }

    private void RespondToRotateInput()
    {
        rigidbody.freezeRotation = true; // take manual control of rotation
        float rotationThisFrame = rcsThrust * Time.deltaTime;

        if (Input.GetKey(KeyCode.A))
        {
            transform.Rotate(Vector3.forward * rotationThisFrame);
        }
        else if (Input.GetKey(KeyCode.D))
        {
            transform.Rotate(-Vector3.forward * rotationThisFrame);
        }

        rigidbody.freezeRotation = false; // resume physics control of rotation 
    }

    private void RespondToThrustInput()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            ApplyThrust();
        }
        else
        {
            audioSource.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust);
        if (!audioSource.isPlaying) //prevents audio from layering
        {
            audioSource.PlayOneShot(mainEnigne);
        }
    }
}
