using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

/// <summary><c>Stealable</c> extends <c>Pickupable</c> to allow the item to
/// be picked up.</summary>
public class Stealable : Pickupable {

    public PickupDestination destination;
    public bool ignoreNextCollision = false;

    public override void Start() {
        taskDescription = "Take the " + this.name;
        base.Start();
    }

    /// <summary> When a stealable item collides with the "endpoint" the item
    /// should be stolen on all clients. </summary>
    void OnCollisionEnter(Collision collision) {
        if (ignoreNextCollision) {
            ignoreNextCollision = false;
        } else if(destination != null && collision.gameObject == destination.gameObject && PhotonNetwork.LocalPlayer.IsMasterClient && task != null) {
            // Calls the steal rpc on all clients
            task.Complete();
	    }
	}

    public override void PrimaryInteraction(Character character) {
        if (!isPickedUp && task != null && task.isCompleted) {
            task.Uncomplete();
        }
        base.PrimaryInteraction(character);

        if (HasTask() && destination != null && character is Loyal) {
            // destination.indicator.SetActive(true);
            destination.EnableTaskMarker();
        }
        if (task != null && destination != null && character is Traitor) {
            destination.EnableDestinationZone();
        }
        DisableTaskMarker();
    }
    
    public override void PrimaryInteractionOff(Character character) {
        Debug.Log($"Dropping");
        base.PrimaryInteractionOff(character);
        // destination.indicator.SetActive(false);
        if (destination != null) {
            destination.DisableTaskMarker();
        }
        if (character is PlayableCharacter) {
            if (task != null && ((PlayableCharacter)character).assignedSubTask == task) {
                EnableTaskMarker();
            }
        }
    }
}
