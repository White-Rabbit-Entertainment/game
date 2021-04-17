using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Rotatingrudder : Sabotageable
{
    public Animator animator;

    [PunRPC]
    public override void Sabotage()
    {
        base.Sabotage();
        animator.speed = 0;
    }

    [PunRPC]
    public override void Fix(int fixPlayerViewId)
    {
        base.Fix(fixPlayerViewId);
        if (!isSabotaged) animator.speed = 1;
    }
}
