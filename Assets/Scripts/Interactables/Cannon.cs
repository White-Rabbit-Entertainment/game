using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Interactable {

    // Cannons will always have 1 cannonball requirement
    CannonBall CannonBall {
        get { return task.requirements[0].GetComponent<CannonBall>(); }
    }

    public override bool CanInteract(Character character) {
        if (character is Traitor && task != null && CannonBall.task.isCompleted) {
            Debug.Log("Cann interact");
            return true;
        }
        Debug.Log("Cannon interact");
        return false;
    }
}
