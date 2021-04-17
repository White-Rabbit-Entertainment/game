using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FillCannon : Pickupable
{
    public override void Reset()
    {
        taskDescription = "Fill cannon by " + this.name;
        base.Reset();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //if(collision.collider.tag = )
        //{

        //}
    }
}
