using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun {
    public CharacterController playerController;
    public float speed = 5f;
    public float sprintFactor = 2f;
    public float stamina = 1f;
    public float staminaDepletionRate = 2;
    public float staminaRegenerationRate = 2;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    private Movement movement;
    public bool frozen = false;

    //Destroy this script if the character is owned by another player
    void Awake() {
        if (photonView != null && !photonView.IsMine) {
            Destroy(this);
        }
    }

    //Create a new movement instance with the given attributes
    void Start() {
        movement = new Movement(speed, gravity, jumpHeight, sprintFactor, stamina, staminaDepletionRate, staminaRegenerationRate);
    }

    void Update() {
        float x = 0;
        float z = 0;
        bool isJumping = false;
        bool isSprinting = false;

        //if the player is not currently frozen read the player inputs
        if (!frozen) {
            x = Input.GetAxis("Horizontal");
            z = Input.GetAxis("Vertical");
            isJumping = Input.GetButtonDown("Jump");
            isSprinting = Input.GetKey("left shift");
        }

        //check if the player is on the ground or in the air
        bool isGrounded = GetComponent<CharacterController>().isGrounded;
        
        //Calculate the movement from the player inputs
        Vector3 move = movement.Calculate(x, z, isJumping, isGrounded, isSprinting, Time.deltaTime);
        Vector3 finalMove = move.x * transform.right + move.y * transform.up + move.z * transform.forward;
        playerController.Move(finalMove);
    }

    //Moves the player to the given position
    [PunRPC] 
    public void MovePlayer(Vector3 position) {

        //Disable the character controller while moving the player
        CharacterController characterController = GetComponent<CharacterController>();
	    characterController.enabled = false;
        transform.position = position;
	    characterController.enabled = true;
    }
}
