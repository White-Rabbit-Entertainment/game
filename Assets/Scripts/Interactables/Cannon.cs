using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PickupDestination))]
public class Cannon : Interactable {

    public override void Reset() {
        base.Reset();
        taskDescription = "Load the cannon";
        softRequirementProbability = 1f;
    }

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

    public bool IsPartOfCannonEndZone(GameObject queryGameObject) {
        if (queryGameObject == gameObject) return true;
        return GetComponent<PickupDestination>().IsPartOfPickUpDestination(queryGameObject);
    }

    public void EnableDestinationZone() {
        GetComponent<PickupDestination>().EnableDestinationZone();
    }
    
    public void DisableDestinationZone() {
        GetComponent<PickupDestination>().DisableDestinationZone();
    }
}
