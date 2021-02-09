using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;
    public CharacterController playerController;
    public float speed = 5f;
    public float staminaStart = 10f;
    public float stamina = 10f;
    public float staminaDepletionRate = 2;
    public float staminaRegenerationRate = 2;
    public float sprintSpeed = 24f;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    private Movement movement;
    private Movement slowMovement;
    private Movement fastMovement;

  
    // Start is called before the first frame update
    void Start()
    {
        slowMovement = new Movement(speed, gravity, jumpHeight);
        fastMovement = new Movement(sprintSpeed, gravity, jumpHeight);
        movement = slowMovement;
    }

    // Update is called once per frame
    void Update()
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
        SprintOrWalk();

    }

    //Decide whether player is walking or sprinting
    void SprintOrWalk() {
        //Left shift triggers sprint if player has stamina
         if (Input.GetKey("left shift"))
        {   
            // print("sprinting");
            if (stamina == 0) {
                //If player hasn't got enough stamina, no sprint
                movement = slowMovement;
                // print("Tired");
            }
            else
            {   
                //Otherwise sprint
                // print("sprinting");
                movement = fastMovement;
            }
            //Deplete stamina because sprint has been attempted (decay function can be swapped out)
            stamina = stamina - (1 / staminaDepletionRate) * Time.deltaTime;
        }
        else 
        {   
            //No sprint attempted so walking, stamina regenerates
            movement = slowMovement;
            stamina = stamina + (1 * staminaRegenerationRate) * Time.deltaTime;
        }
        //Don't let stamina get outside of zero or max value
        this.stamina = Mathf.Clamp(this.stamina, 0, staminaStart);
        // print(stamina);
    }

}
