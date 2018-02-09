using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Rocket : MonoBehaviour {

    Rigidbody rigidBody;
    AudioSource audioSource;
    [SerializeField] float mainThrust = 100f;
    [SerializeField] float rcsThrust = 100f;

    [SerializeField] float levelLoadTime = 1f;

    [SerializeField] float maxLandingVelocity = 10f;
    [SerializeField] float GravityValue = 9.80665f;

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    
    enum State { Alive,Dying, Transcending}
    State state = State.Alive;

    bool collisionsDisabled = false;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();

        Physics.gravity = new Vector3(0, -GravityValue, 0);//Set the value of gravity
    }
	
	// Update is called once per frame
	void Update () {

        if (state == State.Alive)
        {
            Thrust();
            Rotate();
        }
        if (state == State.Dying)
        {
            //if (audioSource.isPlaying)
            //{
            //    audioSource.Stop();
            //}

        }
        if (Debug.isDebugBuild)
        {
            RespondToDebugKeys();
        }


    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up*mainThrust*Time.deltaTime);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }
            mainEngineParticles.Play();

        }
        else
        {
            audioSource.Stop();
            mainEngineParticles.Stop();

        }
    }

    private void Rotate()
    {
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        rigidBody.freezeRotation = true;

        if (Input.GetKey(KeyCode.A))
        {
           
            transform.Rotate(Vector3.forward*rotationThisFrame);


        }
        else if (Input.GetKey(KeyCode.D))
        {

            transform.Rotate(-Vector3.forward * rotationThisFrame);

        }

        rigidBody.freezeRotation = false;
    }

    private void RespondToDebugKeys()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            LoadNextScene();
        }
        else if (Input.GetKeyDown(KeyCode.C))
        {
            collisionsDisabled = !collisionsDisabled; // toggle
        }
    }
        private void OnCollisionEnter(Collision collision)
    {


        if (state != State.Alive || collisionsDisabled)
        {
            return;
        }

        switch (collision.gameObject.tag)
        {
            case "Friendly":
                {
                    if(rigidBody.velocity.magnitude >= maxLandingVelocity)
                    {
                        Die();
                    }
                    break;
                }
            case "Fuel":
                {
                    break;
                }
            case "Finish":
                {
                    if (rigidBody.velocity.magnitude < maxLandingVelocity)
                    {
                        Win();
                    }
                    else
                    {
                        Die();
                    }
                    break;
                }
            default:
                {
                    Die();
                    break;
                }

        }
    }

    private void Win()
    {
        state = State.Transcending;
        audioSource.Stop();
        audioSource.PlayOneShot(winSound);
        winParticles.Play();
        Invoke("LoadNextScene", levelLoadTime);
    }

    private void Die()
    {
        state = State.Dying;
        audioSource.Stop();
        audioSource.PlayOneShot(deathSound);
        deathParticles.Play();
        //Todo: Reanable all rigidbody rotation
        Invoke("ReloadScene", levelLoadTime);
    }

    private void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex+1);
    }
}
