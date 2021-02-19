using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public Transform groundCheck;
    public CharacterController playerController;
    public float speed = 5f;
    public float sprintFactor = 2f;
    public float stamina = 1f;
    public float staminaDepletionRate = 2;
    public float staminaRegenerationRate = 2;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    private Movement movement;
    

    void Awake() {
        // If the player is not me (ie not some other player on the network)
        // then destory this script
        if (!photonView.IsMine) {
            Destroy(this);
        }

        // Dont destory a player on scene change
        // DontDestroyOnLoad(gameObject);
    }
  
    // Start is called before the first frame update
    void Start() {
        movement = new Movement(speed, gravity, jumpHeight, sprintFactor, stamina, staminaDepletionRate, staminaRegenerationRate);
    }

    [PunRPC]
    public void MovePlayer(Vector3 position) {
        CharacterController characterController = GetComponent<CharacterController>();
	    characterController.enabled = false;
        transform.position = position;
	    characterController.enabled = true;
    }

    bool IsGrounded() {
        float colliderHeight = GetComponent<Collider>().bounds.extents.y;
        return Physics.Raycast(transform.position,Vector3.down, colliderHeight + 0.1f);
    }

    // Update is called once per frame
    void Update() {
        //get user input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");
        bool isGrounded = IsGrounded();
        bool isSprinting = Input.GetKey("left shift");
        //apply movement
        Vector3 move = movement.Calculate(x, z, isJumping, isGrounded, isSprinting, Time.deltaTime);
        Vector3 finalMove = move.x * transform.right + move.y * transform.up + move.z * transform.forward;
        playerController.Move(finalMove);

    }

}
