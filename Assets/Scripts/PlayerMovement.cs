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
    public float stamina = 1f;
    public float staminaDepletionRate = 2;
    public float staminaRegenerationRate = 2;
    public float gravity = -19.62f;
    public float jumpHeight = 3f;
    private Movement movement;


  
    // Start is called before the first frame update
    void Start()
    {
        movement = new Movement(speed, gravity, jumpHeight);
    }

    // Update is called once per frame
    void Update()
    {
        //get user input
        float x = Input.GetAxis("Horizontal");
        float z = Input.GetAxis("Vertical");
        bool isJumping = Input.GetButtonDown("Jump");
        bool isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);
        bool isSprinting = Input.GetKey("left shift");
        isSprinting = staminaHandler(isSprinting);
        //apply movement
        Vector3 move = movement.Calculate(x, z, isJumping, isGrounded, isSprinting, Time.deltaTime);
        Vector3 finalMove = move.x * transform.right + move.y * transform.up + move.z * transform.forward;
        playerController.Move(finalMove);

    }

    //Decide whether player is walking or sprinting
    bool staminaHandler(bool isSprinting) {
        //Left shift triggers sprint if player has stamina
         if (isSprinting)
        {   
            if (stamina == 0) {
                //If player hasn't got enough stamina, no sprint
                return false;
            }
            //Deplete stamina because sprint has been attempted (decay function can be swapped out)
            stamina = stamina - (1 / staminaDepletionRate) * Time.deltaTime;
        }
        else 
        {   
            //No sprint attempted so walking, stamina regenerates
            stamina = stamina + (1 * staminaRegenerationRate) * Time.deltaTime;
            //Don't let stamina get outside of zero or max value
            this.stamina = Mathf.Clamp(this.stamina, 0, 1);
            return false;
        }
        //Don't let stamina get outside of zero or max value
        this.stamina = Mathf.Clamp(this.stamina, 0, 1);
        // print(stamina);
        return true;
    }

}
