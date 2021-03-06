﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FootstepSounds : MonoBehaviour
{
    public CharacterController playerController;
    public AudioSource audioSource;

    // Start is called before the first frame update
    void Start()
    {   
        //Assign audiosource
        audioSource = gameObject.GetComponent<AudioSource> ();
    }

    // Update is called once per frame
    void Update()
    {   
        //If player is grounded and moving then play sound
        //High pitched sound for player running fast
        //Random ranges of pitch and volume to add variation to step sounds
        if (playerController.isGrounded && Input.GetKey("left shift") && !audioSource.isPlaying) {
            GetComponent<AudioSource>().volume = Random.Range(0.2f,0.3f);
            GetComponent<AudioSource>().pitch = Random.Range(1,1.2f) + 0.02f * playerController.velocity.magnitude;
            audioSource.PlayOneShot(audioSource.clip, 1); 
        }
        //Low pitched sound for player running slow
        else if (playerController.isGrounded && playerController.velocity.magnitude > 0 && !audioSource.isPlaying) {
            GetComponent<AudioSource>().volume = Random.Range(0.1f,0.2f);
            GetComponent<AudioSource>().pitch = Random.Range(0.8f,1.2f);
            audioSource.PlayOneShot(audioSource.clip, 1); 
        }
    }
}
