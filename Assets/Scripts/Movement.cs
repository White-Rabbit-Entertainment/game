using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Movement
{
    private float groundDistance = 0.4f;
    public float speed;
    public float gravity;
    public float jumpHeight;
    public float y = 0f;

    public Movement(float speed, float gravity, float jumpHeight) {
        this.speed = speed;
        this.gravity = gravity;
        this.jumpHeight = jumpHeight;
    }

    public Vector3 Calculate(float x, float z, bool isJumping, bool isGrounded, float deltaTime) {

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
        move.y = y;  //adding gravity transform

        return move * deltaTime;
    }
}
