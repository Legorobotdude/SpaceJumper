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

    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    [SerializeField] ParticleSystem mainEngineParticles;
    [SerializeField] ParticleSystem deathParticles;
    [SerializeField] ParticleSystem winParticles;

    [SerializeField] bool mobileControls = false;
   

    enum State { Alive,Dying, Transcending}
    State state = State.Alive;

    bool collisionsDisabled = false;

    public bool Thrusting = false;
    public int turning = 0;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
	}
	
	// Update is called once per frame
	void Update () {

        if (state == State.Alive && !mobileControls)
        {
            Thrust();
            Rotate();
        }
        else if (mobileControls)
        {
            if (Thrusting)
            {
                rigidBody.AddRelativeForce(Vector3.up * mainThrust * Time.deltaTime);
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
            if (turning == 1)
            {
                float rotationThisFrame = rcsThrust * Time.deltaTime;

                rigidBody.angularVelocity = Vector3.zero;

                transform.Rotate(Vector3.forward * rotationThisFrame);
            }
            else if (turning == -1)
            {
                float rotationThisFrame = rcsThrust * Time.deltaTime;

                rigidBody.angularVelocity = Vector3.zero;

                transform.Rotate(-Vector3.forward * rotationThisFrame);
            }
        }
        //if (state == State.Dying)
        //{
        //    //if (audioSource.isPlaying)
        //    //{
        //    //    audioSource.Stop();
        //    //}

        //}
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
    public void ThrustMobile()
    {

        Thrusting = true;

        
        //else
        //{
        //    audioSource.Stop();
        //    mainEngineParticles.Stop();

        //}
    }
    public void CancelThrustMobile()
    {

        Thrusting = false;
    }
    private void Rotate()
    {
        
        float rotationThisFrame = rcsThrust * Time.deltaTime;
        //rigidBody.freezeRotation = true;
        rigidBody.angularVelocity = Vector3.zero;

        if (Input.GetKey(KeyCode.A))
        {
           
            transform.Rotate(Vector3.forward*rotationThisFrame);


        }
        else if (Input.GetKey(KeyCode.D))
        {

            transform.Rotate(-Vector3.forward * rotationThisFrame);

        }

        //rigidBody.freezeRotation = false;
    }
    public void RotateLeftMobile()
    {
        turning = 1;
       


    }
    public void RotateRightMobile()
    {

        turning = -1;


    }
    public void cancelTurningMobile()
    {
        turning = 0;
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

                    break;
                }
            case "Fuel":
                {
                    break;
                }
            case "Finish":
                {
                    Win();
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
