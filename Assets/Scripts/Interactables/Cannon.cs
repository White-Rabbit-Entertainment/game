using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// The interactable script for cannon. Cannons combine interaction and pickup
/// destination. This allows the cannon to be the pickup destination for the
/// cannon ball and also allows the cannon to become a task, when the cannon
/// ball is picked up or when it is completed.
[RequireComponent(typeof(PickupDestination))]
public class Cannon : Interactable {

    public override void Reset() {
        base.Reset();
        taskDescription = "Load the cannon";
        // Every cannon always has a CannonBall
        softRequirementProbability = 1f;
    }

    // This property gets the cannon ball that is the requirement of this
    // cannon. Cannons will always have 1 cannonball requirement (i.e. this
    // cannon be null).
    CannonBall CannonBall {
        get { 
            if (task != null && task.requirements.Count > 0) {
                return task.requirements[0].GetComponent<CannonBall>(); 
            }
            return null;
        }
    }

    // Only traitor can interact with cannons and only once the cannon (and
    // cannonball) are completed
    public override bool CanInteract(Character character) {
        if (character is Traitor && CannonBall != null && CannonBall.task.isCompleted) {
            Debug.Log("Cann interact");
            return true;
        }
        Debug.Log("Cannon interact");
        return false;
    }

    // Checks if the provided game object is part of the endzone. Endzones
    // consits of themselves as well as a list of gameobject which can extend
    // the zone.
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
