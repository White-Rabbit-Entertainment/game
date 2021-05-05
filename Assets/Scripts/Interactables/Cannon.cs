using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : Interactable {

    // Cannons will always have 1 cannonball requirement
    CannonBall CannonBall {
        get { 
            if (task != null && task.requirements.Count > 0) {
                return task.requirements[0].GetComponent<CannonBall>(); 
            }
            return null;
        }
    }

    public override bool CanInteract(Character character) {
        if (character is Traitor && CannonBall != null && CannonBall.task.isCompleted) {
            Debug.Log("Cann interact");
            return true;
        }
        Debug.Log("Cannon interact");
        return false;
    }
}
