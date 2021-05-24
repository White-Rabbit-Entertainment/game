using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerAnimation : MonoBehaviourPun {   

    private Animator animator;
    public CharacterController playerController;

    //Destroy this script if the character is owned by another player
    void Awake() {
        if (photonView != null && !photonView.IsMine) {
            Destroy(this);
        }
    }

    //Set initial values for Chrouched and Prone to false
    void Start() {
        animator = GetComponent<Animator>();
        animator.SetBool("Crouched", false);
        animator.SetBool("Prone", false);
    }

    //Update the walking velocity each frame
    void Update() {      
        animator.SetFloat("Walking", GetComponent<Character>().Velocity().magnitude);
    }
}
