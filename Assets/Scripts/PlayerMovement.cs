using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerMovement : MonoBehaviourPun
{
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public CharacterController playerController;
    public float speed = 12f;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    private Movement movement;

    void Awake() {
      DontDestroyOnLoad(gameObject);
    }
  
    // Start is called before the first frame update
    void Start()
    {
        movement = new Movement(speed, gravity, jumpHeight);
    }

    // Update is called once per frame
    void Update() {
      // If the player is me (ie not some other player on the network)
      if (photonView.IsMine) {
        UpdatePosition();
      }
    }

    void UpdatePosition()
    {
        //get user input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        //apply movement
        Vector3 move = movement.Calculate(x, z, isJumping, isGrounded, Time.deltaTime);
        Vector3 finalMove = move.x * transform.right + move.y * transform.up + move.z * transform.forward;
        playerController.Move(finalMove);
    }
}
