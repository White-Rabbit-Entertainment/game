using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateRudder : Interactable
{
    public Animator animator;

    void stopRotate()
    {
        animator.speed = 0;
    }

}
