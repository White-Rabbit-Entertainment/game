using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

// Class for handling movement logic. This uses dependancy injection join avoid
// any external dependancies making it easy to unit test.
public class Movement
{
    // How far from the ground does a player have to be to be "on" the ground.
    private float groundDistance = 0.4f;

    // Player movement speed
    private float speed;

    // Mutliple of speed which a player moves when sprinting
    private float sprintFactor = 2f;

    // Strenght of gravity
    private float gravity;

    // Jump height of character
    private float jumpHeight;

    // Current y position
    private float y = 0f;

    // Stamina
    public float stamina; // Remaining
    private float staminaDepletionRate; // How quickly deplates
    private float staminaRegenerationRate; // How quickly regenerates

    // Init movement class with values
    public Movement(float speed, float gravity, float jumpHeight, float sprintFactor, float stamina, float staminaDepletionRate, float staminaRegenerationRate) {
        this.speed = speed;
        this.gravity = gravity;
        this.jumpHeight = jumpHeight;
        this.sprintFactor = sprintFactor;
        this.stamina = stamina;
        this.staminaDepletionRate = staminaDepletionRate;
        this.staminaRegenerationRate = staminaRegenerationRate;
    }

    // Called every update to determine movement vector required by player
    public Vector3 Calculate(float x, float z, bool isJumping, bool isGrounded, bool isSprinting, float deltaTime) {

        //resets falling velocity if player is grounded
        if (isGrounded && y < 0) {
            y = -2f;
        }

        //makes the player jump if they are on the ground
        if (isJumping && isGrounded) {
            y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        //handles gravity
        y += gravity * deltaTime;

        //combining 2D transform and y velocity
        Vector3 move = new Vector3(x, 0, z) * speed;

        isSprinting = staminaHandler(isSprinting, deltaTime);
        if (isSprinting) move =  move * sprintFactor;
        move.y = y;  //adding gravity transform
        return move * deltaTime;
    }

    // Stamina limits the amount a player can sprint. When sprinting the
    // stamina depletes and when not it replenishes. When a player has no
    // stamina remaining they cant sprint.
    private bool staminaHandler(bool isSprinting, float deltaTime) {
        //Left shift triggers sprint if player has stamina
         if (isSprinting)
        {   
            if (stamina == 0) {
                //If player hasn't got enough stamina, no sprint
                return false;
            }
            //Deplete stamina because sprint has been attempted (decay function can be swapped out)
            stamina = stamina - (1 / staminaDepletionRate) * deltaTime;
            stamina = Mathf.Clamp(stamina, 0, 1);
            return true;
        }
        else 
        {   
            //No sprint attempted so walking, stamina regenerates
            stamina = stamina + (1 / staminaRegenerationRate) * deltaTime;
            //Don't let stamina get outside of zero or max value
            stamina = Mathf.Clamp(stamina, 0, 1);
            return false;
        }
    }
}
