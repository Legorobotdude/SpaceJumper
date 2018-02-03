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
    [SerializeField] AudioClip mainEngine;
    [SerializeField] AudioClip deathSound;
    [SerializeField] AudioClip winSound;

    enum State { Alive,Dying, Transcending}
    State state = State.Alive;

    // Use this for initialization
    void Start () {
        rigidBody = GetComponent<Rigidbody>();
        audioSource = GetComponent<AudioSource>();
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
        
    }

    private void Thrust()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rigidBody.AddRelativeForce(Vector3.up*mainThrust);
            if (!audioSource.isPlaying)
            {
                audioSource.PlayOneShot(mainEngine);
            }


        }
        else
        {
            audioSource.Stop();

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

    private void OnCollisionEnter(Collision collision)
    {


        if (state != State.Alive)
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
                    state = State.Transcending;
                    audioSource.Stop();
                    audioSource.PlayOneShot(winSound);
                    Invoke("LoadNextScene",1f);
                    break;
                }
            default:
                {
                    state = State.Dying;
                    audioSource.Stop();
                    audioSource.PlayOneShot(deathSound);
                    //Die();
                    Invoke("Die", 1f);
                    break;
                }

        }
    }

    private void Die()
    {
        SceneManager.LoadScene(1);
    }

    private void LoadNextScene()
    {
        SceneManager.LoadScene(2);
    }
}
