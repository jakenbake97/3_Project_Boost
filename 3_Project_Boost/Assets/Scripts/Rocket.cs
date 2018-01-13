using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour
{
    /* TODO: 
    *   allow for more than 2 levels
    */

    Rigidbody rigidbody;
    AudioSource audioSource;
    [SerializeField] float rcsThrust = 100f;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float levelLoadDelay = 1f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip levelWin;
    [SerializeField] AudioClip deathExplosion;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem levelWinParticles;
    [SerializeField] ParticleSystem deathExplosionParticles;


    enum State { Alive, Dying, Transcending }
    State state = State.Alive;
    bool collisionsAreDisabled = false;

    // Use this for initialization
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        if (state == State.Alive)
        {
            RespondToRotateInput();
            RespondToThrustInput();
            if (Debug.isDebugBuild)
            {
                RespondToDebugKeys();
            }
        }
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextLevel();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsAreDisabled = !collisionsAreDisabled; //toggle
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (state != State.Alive || collisionsAreDisabled) { return; } // ignore collisons
        switch (collision.gameObject.tag)
        {
            case "Friendly":
                break;
            case "Finish":
                StartLevelWinSequence();
                break;
            default:
                StartDeathSequence();
                break;
        }
    }

    private void StartLevelWinSequence()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(levelWin);
        levelWinParticles.Play();
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartDeathSequence()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathExplosion);
        deathExplosionParticles.Play();
        Invoke("LoadFirstLevel", levelLoadDelay);
    }

    private void LoadFirstLevel()
    {
        SceneManager.LoadScene(0);
    }

    private void LoadNextLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        int nextSceneIndex = currentSceneIndex + 1;
        SceneManager.LoadScene(nextSceneIndex);
        if (currentSceneIndex == (SceneManager.sceneCountInBuildSettings - 1))
        {
            SceneManager.LoadScene(0);
        }

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
            mainEngineParticles.Stop();
        }
    }

    private void ApplyThrust()
    {
        rigidbody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
        if (!audioSource.isPlaying) //prevents audio from layering
        {
            audioSource.PlayOneShot(mainEngine);
        }
        mainEngineParticles.Play();
    }
}
